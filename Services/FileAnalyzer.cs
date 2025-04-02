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
            public double UsedThreshold { get; set; }  // Add new property to store used threshold
            public double DriftValue { get; set; }
            public (DateTime DateTime, double Signal, int Line) MaxSignal { get; set; }
            public (DateTime DateTime, double Signal, int Line) MinSignal { get; set; }
        }

        public AnalysisResult AnalyzeFile(string filePath, double minStdDev, double startSeconds)
        {
            var result = new AnalysisResult();
            var measurements = ParseFile(filePath, result);

            if (!result.IsSuccess || measurements.Count == 0)
                return result;

            CalculateStatistics(measurements, minStdDev, startSeconds, result);
            return result;
        }

        public void SaveResultsToFile(AnalysisResult result, string filePath)
        {
            try
            {
                File.WriteAllLines(filePath, result.Messages);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении файла: {ex.Message}");
            }
        }

        private List<Measurement> ParseFile(string filePath, AnalysisResult result)
        {
            try
            {
                var lines = File.ReadAllLines(filePath).ToList();
                if (lines.Count < 1800)
                {
                    result.Messages.Add("Файл содержит менее 1800 строк. Недостаточно данных для обработки.");
                    result.IsSuccess = false;
                    return new List<Measurement>();
                }

                if (lines.Count < 2)
                {
                    result.Messages.Add("Файл не содержит измерений.");
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
                result.Messages.Add($"Ошибка при чтении файла: {ex.Message}");
                result.IsSuccess = false;
                return new List<Measurement>();
            }
        }

        private void CalculateStatistics(List<Measurement> measurements, double minStdDev, double startSeconds, AnalysisResult result)
        {
            if (measurements.Count < 30)
            {
                result.Messages.Add("Недостаточно данных для расчёта по 30 измерениям.");
                result.IsSuccess = false;
                return;
            }

            result.UsedThreshold = minStdDev;
            int countAboveThreshold = 0;
            var stdDevs = new List<double>();

            for (int i = 29; i < measurements.Count; i++)
            {
                var window = measurements.Skip(i - 29).Take(30).ToList();
                var signals = window.Select(m => m.Signal).ToList();
                var stdDev = CalculateStdDev(signals);

                var lastMeasurement = measurements[i];
                var timeSinceStart = lastMeasurement.DateTime - measurements[0].DateTime;

                if (timeSinceStart.TotalSeconds >= startSeconds)
                {
                    stdDevs.Add(stdDev);
                    result.StdDevValues.Add((lastMeasurement.DateTime, stdDev, timeSinceStart.TotalSeconds));
                    
                    if (stdDev > minStdDev)
                    {
                        countAboveThreshold++;
                        var message = $"{lastMeasurement.DateTime:dd.MM.yyyy HH:mm:ss} ({timeSinceStart.TotalSeconds:F0} секунд) - СКО: {stdDev:F3}";
                        result.ExceededValues.Add(message);
                    }
                }
            }

            result.PercentageAboveThreshold = (double)countAboveThreshold / measurements.Count * 100;
            result.TotalMeasurementTime = measurements.Count;
            result.AverageStdDev = stdDevs.Count > 0 ? stdDevs.Average() : 0;

            // Updated statistics messages to show the actual threshold
            result.GeneralStats.Add($"Процент превышений СКО выше {minStdDev:F2}: {result.PercentageAboveThreshold:F3}%");
            result.GeneralStats.Add($"Общее время измерения сигнала: {result.TotalMeasurementTime} секунд");
            result.GeneralStats.Add($"Среднее значение СКО начиная с {startSeconds} секунд: {result.AverageStdDev:F3}");

            // Calculate drift
            var driftStartIndex = measurements.FindIndex(m => 
                (m.DateTime - measurements[0].DateTime).TotalSeconds >= startSeconds);
            
            var driftEndIndex = measurements.FindIndex(m => 
                (m.DateTime - measurements[0].DateTime).TotalSeconds >= startSeconds + 1800);

            if (driftStartIndex != -1 && driftEndIndex != -1)
            {
                var driftPeriod = measurements.Skip(driftStartIndex).Take(driftEndIndex - driftStartIndex + 1).ToList();
                
                var maxSignal = driftPeriod.MaxBy(m => m.Signal);
                var minSignal = driftPeriod.MinBy(m => m.Signal);
                
                result.MaxSignal = (maxSignal.DateTime, maxSignal.Signal, measurements.IndexOf(maxSignal) + 1);
                result.MinSignal = (minSignal.DateTime, minSignal.Signal, measurements.IndexOf(minSignal) + 1);
                result.DriftValue = Math.Abs(maxSignal.Signal - minSignal.Signal);

                result.GeneralStats.Add("");
                result.GeneralStats.Add($"Расчет дрейфа ({startSeconds}-{startSeconds + 1800} сек.):");
                result.GeneralStats.Add($"Максимум: {result.MaxSignal.Signal:F3}, Время: {result.MaxSignal.DateTime:dd.MM.yyyy HH:mm:ss}, Строка: {result.MaxSignal.Line}");
                result.GeneralStats.Add($"Минимум: {result.MinSignal.Signal:F3}, Время: {result.MinSignal.DateTime:dd.MM.yyyy HH:mm:ss}, Строка: {result.MinSignal.Line}");
                result.GeneralStats.Add($"Значение дрейфа: {result.DriftValue:F3}");
            }
            else
            {
                result.GeneralStats.Add("");
                result.GeneralStats.Add("Недостаточно данных для расчета дрейфа");
            }

            // Объединяем все сообщения для сохранения
            result.Messages.AddRange(result.GeneralStats);
            result.Messages.AddRange(result.ExceededValues);
        }

        private double CalculateStdDev(List<double> values)
        {
            if (values.Count < 2) return 0;
            var mean = values.Average();
            var sumSquares = values.Sum(x => (x - mean) * (x - mean));
            return Math.Sqrt(sumSquares / (values.Count - 1));
        }
    }
}
