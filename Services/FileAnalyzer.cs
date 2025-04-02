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

        private void CalculateStatistics(List<Measurement> measurements, double minStdDev, double startSeconds, AnalysisResult result)
        {
            if (measurements.Count < 30)
            {
                result.Messages.Add("������������ ������ ��� ������� �� 30 ����������.");
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
                        var message = $"{lastMeasurement.DateTime:dd.MM.yyyy HH:mm:ss} ({timeSinceStart.TotalSeconds:F0} ������) - ���: {stdDev:F3}";
                        result.ExceededValues.Add(message);
                    }
                }
            }

            result.PercentageAboveThreshold = (double)countAboveThreshold / measurements.Count * 100;
            result.TotalMeasurementTime = measurements.Count;
            result.AverageStdDev = stdDevs.Count > 0 ? stdDevs.Average() : 0;

            // Updated statistics messages to show the actual threshold
            result.GeneralStats.Add($"������� ���������� ��� ���� {minStdDev:F2}: {result.PercentageAboveThreshold:F3}%");
            result.GeneralStats.Add($"����� ����� ��������� �������: {result.TotalMeasurementTime} ������");
            result.GeneralStats.Add($"������� �������� ��� ������� � {startSeconds} ������: {result.AverageStdDev:F3}");

            // ���������� ��� ��������� ��� ����������
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
