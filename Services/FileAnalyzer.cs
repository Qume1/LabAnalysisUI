using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using LabAnalysisUI.Models;

namespace LabAnalysisUI.Services
{
    public class FileAnalyzer
    {
        public class AnalysisResult
        {
            public List<string> Messages { get; set; } = new();
            public List<string> GeneralStats { get; set; } = new();
            public List<string> ExceededValues { get; set; } = new();
            public double AverageStdDev { get; set; }
            public double PercentageAboveThreshold { get; set; }
            public int TotalMeasurementTime { get; set; }
            public bool IsSuccess { get; set; }
            public List<(DateTime DateTime, double StdDev, double Seconds)> StdDevValues { get; set; } = new();
            public double UsedThreshold { get; set; }  // �������� ����� �������� ��� �������� ��������������� ������
            public double DriftValue { get; set; }
            public (DateTime DateTime, double Signal, int Line) MaxSignal { get; set; }
            public (DateTime DateTime, double Signal, int Line) MinSignal { get; set; }
        }

        // �������� ����� DetectionLimitResult
        public class DetectionLimitResult
        {
            public bool IsSuccess { get; set; }
            public List<string> Messages { get; set; } = new();
            public List<string> AnalysisResults { get; set; } = new();
            public double PercentageAboveThreshold { get; set; }
            public int TotalCount { get; set; }
            public int CountAboveThreshold { get; set; }
        }

        // �������� ����� ����� VirtualSamplesResult
        public class VirtualSamplesResult
        {
            public bool IsSuccess { get; set; }
            public List<string> Messages { get; set; } = new();
            public List<DetectionLimitEntry> DetectionLimits { get; set; } = new();
            public double PercentageAboveThreshold { get; set; }
            public int TotalCount { get; set; }
            public int CountAboveThreshold { get; set; }
        }

        public class DetectionLimitEntry
        {
            public double DetectionLimit { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public int StartLine { get; set; }
            public int EndLine { get; set; }
        }

        public AnalysisResult AnalyzeFile(string filePath, double minStdDev, double startSeconds, double driftStart, double driftEnd)
        {
            var result = new AnalysisResult();
            var measurements = ParseFile(filePath, result);

            if (!result.IsSuccess || measurements.Count == 0)
                return result;

            CalculateStatistics(measurements, minStdDev, startSeconds, driftStart, driftEnd, result);
            return result;
        }

        // �������� ����� ����� ��� ������� ������� ��������������
        public DetectionLimitResult AnalyzeDetectionLimit(string filePath)
        {
            var result = new DetectionLimitResult();
            try
            {
                var lines = File.ReadAllLines(filePath).ToList();
                bool skipFirstThreeLines = true;

                // ��������� ������ ��� ������ �� ������� �����
                for (int i = 0; i < 3 && i < lines.Count; i++)
                {
                    if (Regex.IsMatch(lines[i], @"\d"))
                    {
                        skipFirstThreeLines = false;
                        break;
                    }
                }

                if (skipFirstThreeLines)
                {
                    lines = lines.Skip(3).ToList();
                }

                string pattern = @"(?<value>[-+]?\d+[.,]\d+)\s*(?<date>\d{2}\.\d{2}\.\d{4})\s*(?<time>\d{2}:\d{2}:\d{2})\s*(?<range>(?<min>[-+]?\d+[.,]\d+)\s*-\s*(?<max>[-+]?\d+[.,]\d+))?";
                var regex = new Regex(pattern);

                lines.Reverse(); // ��������� � ����� � ������

                for (int i = 0; i <= lines.Count - 5; i += 5)
                {
                    var values = new List<double>();
                    var timestamps = new List<DateTime>();
                    double minValue = double.MaxValue;
                    double maxValue = double.MinValue;
                    bool hasRange = false;

                    for (int j = i; j < i + 5 && j < lines.Count; j++)
                    {
                        var match = regex.Match(lines[j]);
                        if (match.Success)
                        {
                            string dateStr = match.Groups["date"].Value;
                            string timeStr = match.Groups["time"].Value;
                            string valueStr = match.Groups["value"].Value.Replace(',', '.');
                            string minStr = match.Groups["min"].Value.Replace(',', '.');
                            string maxStr = match.Groups["max"].Value.Replace(',', '.');

                            if (DateTime.TryParseExact(dateStr + " " + timeStr, 
                                "dd.MM.yyyy HH:mm:ss", 
                                CultureInfo.InvariantCulture, 
                                DateTimeStyles.None, 
                                out DateTime dt) &&
                                double.TryParse(valueStr, 
                                    NumberStyles.Any, 
                                    CultureInfo.InvariantCulture, 
                                    out double value))
                            {
                                values.Add(value);
                                timestamps.Add(dt);

                                if (double.TryParse(minStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double min) &&
                                    double.TryParse(maxStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double max))
                                {
                                    minValue = Math.Min(minValue, min);
                                    maxValue = Math.Max(maxValue, max);
                                    hasRange = true;
                                }
                            }
                        }
                    }

                    if (values.Count == 5)
                    {
                        double detectionLimit = CalculateStdDev(values) * 3;
                        DateTime startTime = timestamps.Min();
                        DateTime endTime = timestamps.Max();
                        string timeInterval = $"(� {startTime:HH:mm:ss} �� {endTime:HH:mm:ss})";

                        string analysisResult = hasRange
                            ? $"������ ��������������: {detectionLimit:F3} {timeInterval} {minValue:F2} - {maxValue:F2})"
                            : $"������ ��������������: {detectionLimit:F3} {timeInterval}";

                        result.AnalysisResults.Add(analysisResult);
                        result.TotalCount++;

                        if (detectionLimit > 0.2)
                        {
                            result.CountAboveThreshold++;
                        }
                    }
                }

                if (result.TotalCount > 0)
                {
                    result.PercentageAboveThreshold = (double)result.CountAboveThreshold / result.TotalCount * 100;
                    result.Messages.AddRange(result.AnalysisResults);
                    result.Messages.Add("");
                    result.Messages.Add($"������� ���������� ������� �������������� ���� 0.2: {result.PercentageAboveThreshold:F3}%");
                }
                else
                {
                    result.Messages.Add("��� ������ ��� �������.");
                }

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Messages.Add($"������ ��� �������: {ex.Message}");
            }
            return result;
        }

        // �������� ����� ������ � ����� FileAnalyzer

        public void SaveVirtualSamplesResults(VirtualSamplesResult result, string filePath)
        {
            if (result == null || !result.IsSuccess)
                throw new ArgumentException("��� ������ ��� ����������");

            File.WriteAllLines(filePath, result.Messages);
        }

        public void SaveResultsToFile(AnalysisResult result, string filePath)
        {
            try
            {
                File.WriteAllLines(filePath, result.Messages);
            }
            catch (Exception ex)
            {
                throw new Exception($"������ ��� ���������� �����: {ex.Message}");
            }
        }

        public void SaveDetectionLimitResults(DetectionLimitResult result, string filePath)
        {
            try
            {
                File.WriteAllLines(filePath, result.Messages);
            }
            catch (Exception ex)
            {
                throw new Exception($"������ ��� ���������� �����: {ex.Message}");
            }
        }

        private List<Measurement> ParseFile(string filePath, AnalysisResult result)
        {
            try
            {
                var lines = File.ReadAllLines(filePath).ToList();
                if (lines.Count < 1800)
                {
                    result.Messages.Add("���� �������� ����� 1800 �����. ������������ ������ ��� ���������.");
                    result.IsSuccess = false;
                    return new List<Measurement>();
                }

                if (lines.Count < 2)
                {
                    result.Messages.Add("���� �� �������� ���������.");
                    result.IsSuccess = false;
                    return new List<Measurement>();
                }

                if (Regex.IsMatch(lines[0], @"\d"))
                    lines.RemoveAt(0);

                var pattern = @"^(?<date>\d{2}\.\d{2}\.\d{4})\s+(?<time>\d{2}:\d{2}:\d{2})\s+(?<signal>[-+]?\d+[.,]\d+)";
                var regex = new Regex(pattern);
                var measurements = new List<Measurement>();

                foreach (var line in lines)
                {
                    var match = regex.Match(line);
                    if (match.Success)
                    {
                        var dateStr = match.Groups["date"].Value;
                        var timeStr = match.Groups["time"].Value;
                        var signalStr = match.Groups["signal"].Value.Replace(',', '.');

                        if (DateTime.TryParseExact(dateStr + " " + timeStr, 
                            "dd.MM.yyyy HH:mm:ss", 
                            CultureInfo.InvariantCulture, 
                            DateTimeStyles.None, 
                            out DateTime dt) &&
                            double.TryParse(signalStr, 
                                NumberStyles.Any, 
                                CultureInfo.InvariantCulture, 
                                out double signalVal))
                        {
                            measurements.Add(new Measurement { DateTime = dt, Signal = signalVal });
                        }
                    }
                }

                result.IsSuccess = true;
                return measurements;
            }
            catch (Exception ex)
            {
                result.Messages.Add($"������ ��� ������ �����: {ex.Message}");
                result.IsSuccess = false;
                return new List<Measurement>();
            }
        }
        private List<Measurement> ParseFile(string filePath, VirtualSamplesResult result)
        {
            var analysisResult = new AnalysisResult();
            var measurements = ParseFile(filePath, analysisResult);
            result.IsSuccess = analysisResult.IsSuccess;
            result.Messages.AddRange(analysisResult.Messages);
            return measurements;
        }

        private void CalculateStatistics(List<Measurement> measurements, double minStdDev, double startSeconds, double driftStart, double driftEnd, AnalysisResult result)
        {
            if (measurements.Count < 30)
            {
                result.Messages.Add("������������ ������ ��� ������� �� 30 ����������.");
                result.IsSuccess = false;
                return;
            }

            result.UsedThreshold = minStdDev;
            int countAboveThreshold = 0;
            int totalMeasurementsAfterStart = 0;
            var stdDevs = new List<double>();

            for (int i = 29; i < measurements.Count; i++)
            {
                var window = measurements.Skip(i - 29).Take(30).ToList();
                var signals = window.Select(m => m.Signal).ToList();
                var stdDev = CalculateStdDev(signals);

                var lastMeasurement = measurements[i];
                var timeSinceStart = (lastMeasurement.DateTime - measurements[0].DateTime).TotalSeconds;

                if (timeSinceStart >= startSeconds)  // ������������ startSeconds ��� ������� ���
                {
                    totalMeasurementsAfterStart++;
                    stdDevs.Add(stdDev);
                    result.StdDevValues.Add((lastMeasurement.DateTime, stdDev, timeSinceStart));
                    
                    if (stdDev > minStdDev)
                    {
                        countAboveThreshold++;
                        var message = $"{lastMeasurement.DateTime:dd.MM.yyyy HH:mm:ss} ({timeSinceStart:F0} ������) - ���: {stdDev:F3}";
                        result.ExceededValues.Add(message);
                    }
                }
            }

            // ���������� �������, ��������� ��������� ����� startSeconds
            result.PercentageAboveThreshold = totalMeasurementsAfterStart > 0 
                ? (double)countAboveThreshold / totalMeasurementsAfterStart * 100 
                : 0;

            result.TotalMeasurementTime = measurements.Count;
            result.AverageStdDev = stdDevs.Count > 0 ? stdDevs.Average() : 0;

            result.GeneralStats.Add($"������� ���������� ��� ���� {minStdDev:F2} (������� � {startSeconds} ���.): {result.PercentageAboveThreshold:F3}%");
            result.GeneralStats.Add($"����� ����� ��������� �������: {result.TotalMeasurementTime} ������");
            result.GeneralStats.Add($"������� �������� ��� ������� � {startSeconds} ������: {result.AverageStdDev:F3}");

            // ���������� �����, ��������� driftStart � driftEnd
            var driftStartIndex = measurements.FindIndex(m => (m.DateTime - measurements[0].DateTime).TotalSeconds >= driftStart);
            var driftEndIndex = measurements.FindIndex(m => (m.DateTime - measurements[0].DateTime).TotalSeconds >= driftEnd);

            if (driftStartIndex != -1 && driftEndIndex != -1 && driftEndIndex >= driftStartIndex)
            {
                var driftPeriod = measurements.Skip(driftStartIndex).Take(driftEndIndex - driftStartIndex + 1).ToList();
                
                var maxSignal = driftPeriod.MaxBy(m => m.Signal);
                var minSignal = driftPeriod.MinBy(m => m.Signal);
                
                result.MaxSignal = (maxSignal.DateTime, maxSignal.Signal, measurements.IndexOf(maxSignal) + 1);
                result.MinSignal = (minSignal.DateTime, minSignal.Signal, measurements.IndexOf(minSignal) + 1);
                result.DriftValue = Math.Abs(maxSignal.Signal - minSignal.Signal);

                result.GeneralStats.Add("");
                result.GeneralStats.Add($"������ ������ ({driftStart}-{driftEnd} ���.):");
                result.GeneralStats.Add($"��������: {result.MaxSignal.Signal:F3}, �����: {result.MaxSignal.DateTime:dd.MM.yyyy HH:mm:ss}, ������: {result.MaxSignal.Line}");
                result.GeneralStats.Add($"�������: {result.MinSignal.Signal:F3}, �����: {result.MinSignal.DateTime:dd.MM.yyyy HH:mm:ss}, ������: {result.MinSignal.Line}");
                result.GeneralStats.Add($"�������� ������: {result.DriftValue:F3}");
            }
            else
            {
                result.GeneralStats.Add("");
                result.GeneralStats.Add("������������ ������ ��� ������� ������");
            }

            result.Messages.AddRange(result.GeneralStats);
            result.Messages.AddRange(result.ExceededValues);
        }

        private static double CalculateStdDev(List<double> values)
        {
            if (values.Count < 2) return 0;
            var mean = values.Average();
            var sumSquares = values.Sum(x => (x - mean) * (x - mean));
            return Math.Sqrt(sumSquares / (values.Count - 1));
        }
        private static double CalculateRegressionLine(List<Measurement> beforeSegment, List<Measurement> firstSegment, List<Measurement> lastSegment, List<Measurement> afterSegment)
        {
            double AverageOrZero(List<Measurement> segment) => segment.Count > 0 ? segment.Average(m => m.Signal) : 0;

            double firstPointY = (AverageOrZero(beforeSegment) + AverageOrZero(firstSegment)) / 2;
            double secondPointY = (AverageOrZero(lastSegment) + AverageOrZero(afterSegment)) / 2;

            double firstPointX = 0;
            double secondPointX = 1;

            double slope = (secondPointY - firstPointY) / (secondPointX - firstPointX);
            double intercept = firstPointY - slope * firstPointX;

            return slope * 0.5 + intercept;
        }

        private static double CalculateSignedArea(List<Measurement> segment, double regressionLine)
        {
            double area = 0;
            foreach (var measurement in segment)
            {
                area += (measurement.Signal - regressionLine);
            }
            return area;
        }

        public VirtualSamplesResult AnalyzeVirtualSamples(string filePath, double calibrationCoef = 252.1, int intervalSize = 60)
        {
            var result = new VirtualSamplesResult();
            try
            {
                var measurements = ParseFile(filePath, result);

                if (measurements.Count < 1800 + intervalSize + 18)
                {
                    result.Messages.Add($"������������ ������ ��� ������� �� {intervalSize} ���������� ������� � 1800 ������.");
                    return result;
                }

                List<double> areas = new List<double>();

                for (int i = 1800; i <= measurements.Count - intervalSize; i += (intervalSize + 18))
                {
                    var segment = measurements.Skip(i).Take(intervalSize).ToList();
                    var beforeSegment = measurements.Skip(i - 9).Take(9).ToList();
                    var firstSegment = measurements.Skip(i).Take(1).ToList();
                    var lastSegment = measurements.Skip(i + intervalSize - 1).Take(1).ToList();
                    var afterSegment = measurements.Skip(i + intervalSize).Take(9).ToList();

                    double regressionLine = CalculateRegressionLine(beforeSegment, firstSegment, lastSegment, afterSegment);
                    double area = CalculateSignedArea(segment, regressionLine) / calibrationCoef;
                    areas.Add(area);
                }

                for (int i = 0; i <= areas.Count - 5; i += 5)
                {
                    var group = areas.Skip(i).Take(5).ToList();
                    if (group.Count == 5)
                    {
                        double stdDev = CalculateStdDev(group) * 3;
                        DateTime startTime = measurements[1800 + i * (intervalSize + 18)].DateTime;
                        DateTime endTime = measurements[1800 + (i + 4) * (intervalSize + 18) + (intervalSize - 1)].DateTime;
                        int startLine = 1800 + i * (intervalSize + 18) + 1;
                        int endLine = 1800 + (i + 4) * (intervalSize + 18) + intervalSize;

                        var entry = new DetectionLimitEntry
                        {
                            DetectionLimit = stdDev,
                            StartTime = startTime,
                            EndTime = endTime,
                            StartLine = startLine,
                            EndLine = endLine
                        };

                        result.DetectionLimits.Add(entry);
                        result.TotalCount++;

                        if (stdDev > 0.2)
                        {
                            result.CountAboveThreshold++;
                        }
                    }
                }

                result.PercentageAboveThreshold = (double)result.CountAboveThreshold / result.TotalCount * 100;
                result.IsSuccess = true;

                foreach (var limit in result.DetectionLimits)
                {
                    result.Messages.Add($"������ ��������������: {limit.DetectionLimit:F3} (� {limit.StartTime:HH:mm:ss} �� {limit.EndTime:HH:mm:ss}) (������: {limit.StartLine} - {limit.EndLine})");
                }

                result.Messages.Add($"������� ���������� ������� �������������� ���� 0.2: {result.PercentageAboveThreshold:F3}%");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Messages.Add($"������ ��� �������: {ex.Message}");
            }

            return result;
        }
    }
}
