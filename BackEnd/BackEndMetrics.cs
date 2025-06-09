namespace MsSentinel.BanffProtect.Backend.Metrics;

using System.Diagnostics.Metrics;

// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-instrumentation#get-a-meter-via-dependency-injection
public class BackEndMetrics
{
    private readonly Counter<int> _logsSent;
    private readonly Counter<int> _logLinesSent;

    public BackEndMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("MsSentinel.BanffProtect");
        _logsSent = meter.CreateCounter<int>("backend.logs.sent");
        _logLinesSent = meter.CreateCounter<int>("backend.logs.lines.sent");
    }

    public void LogsSent(int quantity, string stream)
    {
        _logsSent.Add(quantity, new KeyValuePair<string,object?>("stream", stream));
    }

    public void LogLinesSent(int quantity, string stream)
    {
        _logLinesSent.Add(quantity, new KeyValuePair<string,object?>("stream", stream));
    }
}
