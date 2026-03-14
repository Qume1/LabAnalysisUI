namespace LabAnalysisUI.Models;

public sealed class Measurement
{
    public DateTime DateTime { get; set; }
    public double Signal { get; set; }
}

public sealed class AnalysisResult
{
    public bool IsSuccess { get; set; }
    public List<string> Messages { get; } = new();
    public List<string> GeneralStats { get; } = new();
    public List<string> ExceededValues { get; } = new();
    public List<StdDevPoint> StdDevValues { get; } = new();
    public double AverageStdDev { get; set; }
    public double PercentageAboveThreshold { get; set; }
    public int TotalMeasurementTime { get; set; }
    public double UsedThreshold { get; set; }
    public double DriftValue { get; set; }
    public SignalPoint? MaxSignal { get; set; }
    public SignalPoint? MinSignal { get; set; }
}

public sealed class DetectionLimitResult
{
    public bool IsSuccess { get; set; }
    public List<string> Messages { get; } = new();
    public List<string> AnalysisResults { get; } = new();
    public double PercentageAboveThreshold { get; set; }
    public int TotalCount { get; set; }
    public int CountAboveThreshold { get; set; }
}

public sealed class VirtualSamplesResult
{
    public bool IsSuccess { get; set; }
    public List<string> Messages { get; } = new();
    public List<DetectionLimitEntry> DetectionLimits { get; } = new();
    public double PercentageAboveThreshold { get; set; }
    public int TotalCount { get; set; }
    public int CountAboveThreshold { get; set; }
}

public sealed class DetectionLimitEntry
{
    public double DetectionLimit { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int StartLine { get; set; }
    public int EndLine { get; set; }
}

public readonly record struct StdDevPoint(DateTime DateTime, double StdDev, double Seconds);

public readonly record struct SignalPoint(DateTime DateTime, double Signal, int Line);

