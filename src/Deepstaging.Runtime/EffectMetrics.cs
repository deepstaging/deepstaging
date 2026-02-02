using System.Diagnostics.Metrics;

namespace Deepstaging;

/// <summary>
/// OpenTelemetry metrics for effect operations.
/// </summary>
public sealed class EffectMetrics : IDisposable
{
    private readonly Meter _meter;
    private readonly Counter<long> _operationsSucceeded;
    private readonly Counter<long> _operationsFailed;
    private readonly Histogram<double> _operationDuration;
    
    /// <summary>
    /// Creates a new EffectMetrics instance with the specified meter name.
    /// </summary>
    /// <param name="meterName">The name of the meter (e.g., "Deepstaging.Effects").</param>
    public EffectMetrics(string meterName = "Deepstaging.Effects")
    {
        _meter = new Meter(meterName, "1.0.0");
        
        _operationsSucceeded = _meter.CreateCounter<long>(
            "deepstaging.effects.operations.succeeded",
            "operations",
            "Number of successful effect operations");
            
        _operationsFailed = _meter.CreateCounter<long>(
            "deepstaging.effects.operations.failed", 
            "operations",
            "Number of failed effect operations");
            
        _operationDuration = _meter.CreateHistogram<double>(
            "deepstaging.effects.operations.duration",
            "ms",
            "Duration of effect operations in milliseconds");
    }
    
    /// <summary>
    /// Records a successful operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="durationMs">The duration in milliseconds.</param>
    /// <param name="tags">Optional additional tags.</param>
    public void RecordSuccess(string operationName, double durationMs, params KeyValuePair<string, object?>[] tags)
    {
        var allTags = CreateTags(operationName, tags);
        _operationsSucceeded.Add(1, allTags);
        _operationDuration.Record(durationMs, allTags);
    }
    
    /// <summary>
    /// Records a failed operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="durationMs">The duration in milliseconds.</param>
    /// <param name="errorType">The type of error that occurred.</param>
    /// <param name="tags">Optional additional tags.</param>
    public void RecordFailure(string operationName, double durationMs, string? errorType = null, params KeyValuePair<string, object?>[] tags)
    {
        var allTags = CreateTags(operationName, tags);
        if (errorType != null)
        {
            allTags = [..allTags, new KeyValuePair<string, object?>("error.type", errorType)];
        }
        _operationsFailed.Add(1, allTags);
        _operationDuration.Record(durationMs, allTags);
    }
    
    private static KeyValuePair<string, object?>[] CreateTags(string operationName, KeyValuePair<string, object?>[] additionalTags)
    {
        var baseTags = new[] { new KeyValuePair<string, object?>("operation", operationName) };
        return additionalTags.Length > 0 ? [..baseTags, ..additionalTags] : baseTags;
    }
    
    /// <inheritdoc/>
    public void Dispose()
    {
        _meter.Dispose();
    }
}
