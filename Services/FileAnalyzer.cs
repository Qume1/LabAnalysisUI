using System.Globalization;
using System.Text.RegularExpressions;
using LabAnalysisUI.Models;

namespace LabAnalysisUI.Services;

public sealed class FileAnalyzer
{
    private const int MinimumMeasurementCount = 3600;
    private const int RollingWindowSize = 30;
    private const int DetectionGroupSize = 5;
    private const double DetectionThreshold = 0.2;
    private const int VirtualSamplesStartIndex = 1800;
    private const int VirtualSamplePadding = 9;

    private static readonly Regex MeasurementRegex = new(
        @"^(?<date>\d{2}\.\d{2}\.\d{4})\s+(?<time>\d{2}:\d{2}:\d{2})\s+(?<signal>[-+]?\d+[.,]\d+)",
        RegexOptions.Compiled);

    private static readonly Regex DetectionLimitRegex = new(
        @"(?<value>[-+]?\d+[.,]\d+)\s*(?<date>\d{2}\.\d{2}\.\d{4})\s*(?<time>\d{2}:\d{2}:\d{2})\s*(?:(?<min>[-+]?\d+[.,]\d+)\s*-\s*(?<max>[-+]?\d+[.,]\d+))?",
        RegexOptions.Compiled);

    public AnalysisResult AnalyzeFile(string filePath, double minStdDev, double startSeconds, double driftStart, double driftEnd)
    {
        var result = new AnalysisResult();
        var measurements = ParseMeasurements(
            filePath,
            result.Messages,
            MinimumMeasurementCount,
            "Недостаточно данных для анализа. Требуется минимум 3600 строк измерений.");

        if (measurements.Count == 0)
        {
            result.IsSuccess = false;
            return result;
        }

        CalculateStatistics(measurements, minStdDev, startSeconds, driftStart, driftEnd, result);

        if (result.IsSuccess)
        {
            result.Messages.Clear();
            result.Messages.AddRange(CreateAnalysisReport(result, includeExceededValues: true));
        }

        return result;
    }

    public DetectionLimitResult AnalyzeDetectionLimit(string filePath)
    {
        var result = new DetectionLimitResult();

        try
        {
            var lines = ReadLines(filePath, result.Messages);
            if (lines.Count == 0)
            {
                result.IsSuccess = false;
                return result;
            }

            var preparedLines = TrimLeadingHeader(lines, 3);
            var entries = ParseDetectionLimitEntries(preparedLines);
            entries.Reverse();

            foreach (var group in entries.Chunk(DetectionGroupSize))
            {
                if (group.Length < DetectionGroupSize)
                {
                    continue;
                }

                var detectionLimit = CalculateStdDev(group.Select(entry => entry.Value).ToList()) * 3;
                var startTime = group.Min(entry => entry.Timestamp);
                var endTime = group.Max(entry => entry.Timestamp);
                var minRange = group.Where(entry => entry.Minimum.HasValue).Select(entry => entry.Minimum!.Value).DefaultIfEmpty().Min();
                var maxRange = group.Where(entry => entry.Maximum.HasValue).Select(entry => entry.Maximum!.Value).DefaultIfEmpty().Max();
                var hasRange = group.Any(entry => entry.Minimum.HasValue && entry.Maximum.HasValue);

                var message = hasRange
                    ? $"Предел детектирования: {detectionLimit:F3} (с {startTime:HH:mm:ss} по {endTime:HH:mm:ss}) | Диапазон: {minRange:F2} - {maxRange:F2}"
                    : $"Предел детектирования: {detectionLimit:F3} (с {startTime:HH:mm:ss} по {endTime:HH:mm:ss})";

                result.AnalysisResults.Add(message);
                result.TotalCount++;

                if (detectionLimit > DetectionThreshold)
                {
                    result.CountAboveThreshold++;
                }
            }

            if (result.TotalCount == 0)
            {
                result.Messages.Add("Недостаточно валидных данных для расчета предела детектирования.");
            }
            else
            {
                result.PercentageAboveThreshold = (double)result.CountAboveThreshold / result.TotalCount * 100;
                result.Messages.AddRange(result.AnalysisResults);
                result.Messages.Add(string.Empty);
                result.Messages.Add($"Процент превышений предела детектирования выше {DetectionThreshold:F1}: {result.PercentageAboveThreshold:F3}%");
            }

            result.IsSuccess = true;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Messages.Clear();
            result.Messages.Add($"Ошибка при анализе: {ex.Message}");
        }

        return result;
    }

    public VirtualSamplesResult AnalyzeVirtualSamples(string filePath, double calibrationCoef = 252.1, int intervalSize = 60)
    {
        var result = new VirtualSamplesResult();

        if (calibrationCoef <= 0)
        {
            result.Messages.Add("Калибровочный коэффициент должен быть больше нуля.");
            result.IsSuccess = false;
            return result;
        }

        var minimumCount = VirtualSamplesStartIndex + intervalSize + (VirtualSamplePadding * 2);
        var measurements = ParseMeasurements(
            filePath,
            result.Messages,
            minimumCount,
            $"Недостаточно данных для расчета виртуальных проб. Требуется минимум {minimumCount} измерений.");

        if (measurements.Count == 0)
        {
            result.IsSuccess = false;
            return result;
        }

        var step = intervalSize + (VirtualSamplePadding * 2);
        var areas = new List<double>();
        var segmentStarts = new List<int>();

        for (var index = VirtualSamplesStartIndex; index <= measurements.Count - intervalSize - VirtualSamplePadding; index += step)
        {
            var segment = measurements.Skip(index).Take(intervalSize).ToList();
            var beforeSegment = measurements.Skip(index - VirtualSamplePadding).Take(VirtualSamplePadding).ToList();
            var firstPoint = measurements.Skip(index).Take(1).ToList();
            var lastPoint = measurements.Skip(index + intervalSize - 1).Take(1).ToList();
            var afterSegment = measurements.Skip(index + intervalSize).Take(VirtualSamplePadding).ToList();

            if (segment.Count < intervalSize || beforeSegment.Count < VirtualSamplePadding || afterSegment.Count < VirtualSamplePadding)
            {
                break;
            }

            var regressionLine = CalculateRegressionLine(beforeSegment, firstPoint, lastPoint, afterSegment);
            var area = CalculateSignedArea(segment, regressionLine) / calibrationCoef;

            areas.Add(area);
            segmentStarts.Add(index);
        }

        for (var index = 0; index <= areas.Count - DetectionGroupSize; index += DetectionGroupSize)
        {
            var group = areas.Skip(index).Take(DetectionGroupSize).ToList();
            if (group.Count < DetectionGroupSize)
            {
                continue;
            }

            var detectionLimit = CalculateStdDev(group) * 3;
            var startIndex = segmentStarts[index];
            var endIndex = segmentStarts[index + DetectionGroupSize - 1] + intervalSize - 1;

            result.DetectionLimits.Add(new DetectionLimitEntry
            {
                DetectionLimit = detectionLimit,
                StartTime = measurements[startIndex].DateTime,
                EndTime = measurements[endIndex].DateTime,
                StartLine = startIndex + 1,
                EndLine = endIndex + 1
            });

            result.TotalCount++;

            if (detectionLimit > DetectionThreshold)
            {
                result.CountAboveThreshold++;
            }
        }

        result.PercentageAboveThreshold = result.TotalCount > 0
            ? (double)result.CountAboveThreshold / result.TotalCount * 100
            : 0;

        if (result.DetectionLimits.Count == 0)
        {
            result.Messages.Add("Недостаточно сегментов для расчета по виртуальным пробам.");
            result.IsSuccess = false;
            return result;
        }

        foreach (var limit in result.DetectionLimits)
        {
            result.Messages.Add($"Предел детектирования: {limit.DetectionLimit:F3} (с {limit.StartTime:HH:mm:ss} по {limit.EndTime:HH:mm:ss}) | Строки: {limit.StartLine} - {limit.EndLine}");
        }

        result.Messages.Add(string.Empty);
        result.Messages.Add($"Процент превышений предела детектирования выше {DetectionThreshold:F1}: {result.PercentageAboveThreshold:F3}%");
        result.IsSuccess = true;
        return result;
    }

    public static IReadOnlyList<string> CreateAnalysisReport(AnalysisResult result, bool includeExceededValues)
    {
        var lines = new List<string>();
        lines.AddRange(result.GeneralStats);

        if (includeExceededValues && result.ExceededValues.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add($"Значения СКО, превышающие {result.UsedThreshold:F2}:");
            lines.AddRange(result.ExceededValues);
        }

        return lines;
    }

    private static List<Measurement> ParseMeasurements(string filePath, List<string> messages, int minimumRequiredLines, string insufficientDataMessage)
    {
        var lines = ReadLines(filePath, messages);
        if (lines.Count == 0)
        {
            return new List<Measurement>();
        }

        if (lines.Count < minimumRequiredLines)
        {
            messages.Add(insufficientDataMessage);
            return new List<Measurement>();
        }

        var measurements = new List<Measurement>(lines.Count);

        foreach (var line in lines)
        {
            var match = MeasurementRegex.Match(line);
            if (!match.Success)
            {
                continue;
            }

            if (!TryParseMeasurement(match, out var measurement))
            {
                continue;
            }

            measurements.Add(measurement);
        }

        if (measurements.Count == 0)
        {
            messages.Add("Не удалось распознать измерения в выбранном файле.");
        }

        return measurements;
    }

    private static void CalculateStatistics(List<Measurement> measurements, double minStdDev, double startSeconds, double driftStart, double driftEnd, AnalysisResult result)
    {
        if (measurements.Count < RollingWindowSize)
        {
            result.Messages.Add("Недостаточно данных для расчета СКО.");
            result.IsSuccess = false;
            return;
        }

        result.IsSuccess = true;
        result.UsedThreshold = minStdDev;

        var countAboveThreshold = 0;
        var totalMeasurementsAfterStart = 0;
        var stdDevs = new List<double>();

        for (var index = RollingWindowSize - 1; index < measurements.Count; index++)
        {
            var window = measurements.Skip(index - (RollingWindowSize - 1)).Take(RollingWindowSize).Select(measurement => measurement.Signal).ToList();
            var stdDev = CalculateStdDev(window);
            var lastMeasurement = measurements[index];
            var timeSinceStart = (lastMeasurement.DateTime - measurements[0].DateTime).TotalSeconds;

            if (timeSinceStart < startSeconds)
            {
                continue;
            }

            totalMeasurementsAfterStart++;
            stdDevs.Add(stdDev);
            result.StdDevValues.Add(new StdDevPoint(lastMeasurement.DateTime, stdDev, timeSinceStart));

            if (stdDev <= minStdDev)
            {
                continue;
            }

            countAboveThreshold++;
            result.ExceededValues.Add($"{lastMeasurement.DateTime:dd.MM.yyyy HH:mm:ss} ({timeSinceStart:F0} секунд) - СКО: {stdDev:F3}");
        }

        result.PercentageAboveThreshold = totalMeasurementsAfterStart > 0
            ? (double)countAboveThreshold / totalMeasurementsAfterStart * 100
            : 0;
        result.TotalMeasurementTime = (int)(measurements[^1].DateTime - measurements[0].DateTime).TotalSeconds;
        result.AverageStdDev = stdDevs.Count > 0 ? stdDevs.Average() : 0;

        result.GeneralStats.Add($"Процент превышений СКО выше {minStdDev:F2} (начиная с {startSeconds:F0} сек.): {result.PercentageAboveThreshold:F3}%");
        result.GeneralStats.Add($"Длительность измерения сигнала: {result.TotalMeasurementTime} секунд");
        result.GeneralStats.Add($"Среднее значение СКО начиная с {startSeconds:F0} секунд: {result.AverageStdDev:F3}");

        AppendDriftStatistics(measurements, driftStart, driftEnd, result);
    }

    private static void AppendDriftStatistics(List<Measurement> measurements, double driftStart, double driftEnd, AnalysisResult result)
    {
        var driftStartIndex = measurements.FindIndex(measurement => (measurement.DateTime - measurements[0].DateTime).TotalSeconds >= driftStart);
        var driftEndIndex = measurements.FindIndex(measurement => (measurement.DateTime - measurements[0].DateTime).TotalSeconds >= driftEnd);

        if (driftStartIndex < 0 || driftEndIndex < driftStartIndex)
        {
            result.GeneralStats.Add(string.Empty);
            result.GeneralStats.Add("Недостаточно данных для расчета дрейфа.");
            return;
        }

        var driftPeriod = measurements.Skip(driftStartIndex).Take(driftEndIndex - driftStartIndex + 1).ToList();
        var maxSignal = driftPeriod.MaxBy(measurement => measurement.Signal);
        var minSignal = driftPeriod.MinBy(measurement => measurement.Signal);

        if (maxSignal is null || minSignal is null)
        {
            result.GeneralStats.Add(string.Empty);
            result.GeneralStats.Add("Недостаточно данных для расчета дрейфа.");
            return;
        }

        result.MaxSignal = new SignalPoint(maxSignal.DateTime, maxSignal.Signal, measurements.IndexOf(maxSignal) + 1);
        result.MinSignal = new SignalPoint(minSignal.DateTime, minSignal.Signal, measurements.IndexOf(minSignal) + 1);
        result.DriftValue = Math.Abs(maxSignal.Signal - minSignal.Signal);

        result.GeneralStats.Add(string.Empty);
        result.GeneralStats.Add($"Расчет дрейфа ({driftStart:F0}-{driftEnd:F0} сек.):");
        result.GeneralStats.Add($"Максимум: {maxSignal.Signal:F3}, Время: {maxSignal.DateTime:dd.MM.yyyy HH:mm:ss}, Строка: {result.MaxSignal.Value.Line}");
        result.GeneralStats.Add($"Минимум: {minSignal.Signal:F3}, Время: {minSignal.DateTime:dd.MM.yyyy HH:mm:ss}, Строка: {result.MinSignal.Value.Line}");
        result.GeneralStats.Add($"Значение дрейфа: {result.DriftValue:F3}");
    }

    private static List<string> ReadLines(string filePath, List<string> messages)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                messages.Add("Файл не найден.");
                return new List<string>();
            }

            return File.ReadLines(filePath).Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        }
        catch (Exception ex)
        {
            messages.Add($"Ошибка при чтении файла: {ex.Message}");
            return new List<string>();
        }
    }

    private static List<string> TrimLeadingHeader(IReadOnlyList<string> lines, int headerLineCount)
    {
        var header = lines.Take(headerLineCount).ToList();
        return header.Any(line => line.Any(char.IsDigit))
            ? lines.ToList()
            : lines.Skip(headerLineCount).ToList();
    }

    private static List<DetectionSample> ParseDetectionLimitEntries(IEnumerable<string> lines)
    {
        var entries = new List<DetectionSample>();

        foreach (var line in lines)
        {
            var match = DetectionLimitRegex.Match(line);
            if (!match.Success)
            {
                continue;
            }

            if (!DateTime.TryParseExact(
                    $"{match.Groups["date"].Value} {match.Groups["time"].Value}",
                    "dd.MM.yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var timestamp))
            {
                continue;
            }

            if (!TryParseFlexibleDouble(match.Groups["value"].Value, out var value))
            {
                continue;
            }

            double? minimum = TryParseFlexibleDouble(match.Groups["min"].Value, out var minValue) ? minValue : null;
            double? maximum = TryParseFlexibleDouble(match.Groups["max"].Value, out var maxValue) ? maxValue : null;
            entries.Add(new DetectionSample(value, timestamp, minimum, maximum));
        }

        return entries;
    }

    private static bool TryParseMeasurement(Match match, out Measurement measurement)
    {
        measurement = new Measurement();

        if (!DateTime.TryParseExact(
                $"{match.Groups["date"].Value} {match.Groups["time"].Value}",
                "dd.MM.yyyy HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dateTime))
        {
            return false;
        }

        if (!TryParseFlexibleDouble(match.Groups["signal"].Value, out var signal))
        {
            return false;
        }

        measurement = new Measurement
        {
            DateTime = dateTime,
            Signal = signal
        };

        return true;
    }

    private static bool TryParseFlexibleDouble(string rawValue, out double value)
    {
        var normalizedValue = rawValue.Replace(',', '.');
        return double.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
    }

    private static double CalculateStdDev(IReadOnlyList<double> values)
    {
        if (values.Count < 2)
        {
            return 0;
        }

        var mean = values.Average();
        var sumSquares = values.Sum(value => Math.Pow(value - mean, 2));
        return Math.Sqrt(sumSquares / (values.Count - 1));
    }

    private static double CalculateRegressionLine(List<Measurement> beforeSegment, List<Measurement> firstSegment, List<Measurement> lastSegment, List<Measurement> afterSegment)
    {
        static double AverageOrZero(List<Measurement> segment) => segment.Count > 0 ? segment.Average(measurement => measurement.Signal) : 0;

        var firstPointY = (AverageOrZero(beforeSegment) + AverageOrZero(firstSegment)) / 2;
        var secondPointY = (AverageOrZero(lastSegment) + AverageOrZero(afterSegment)) / 2;
        var slope = secondPointY - firstPointY;
        var intercept = firstPointY;

        return (slope * 0.5) + intercept;
    }

    private static double CalculateSignedArea(List<Measurement> segment, double regressionLine)
    {
        return segment.Sum(measurement => measurement.Signal - regressionLine);
    }

    private sealed record DetectionSample(double Value, DateTime Timestamp, double? Minimum, double? Maximum);
}

