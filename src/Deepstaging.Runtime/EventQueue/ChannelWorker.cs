// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.Extensions.Hosting;

namespace Deepstaging.EventQueue;

/// <summary>
/// Base class for event queue workers that read from an <see cref="EventQueueChannel{TEvent}"/>
/// and dispatch events to handlers. Generated queue services inherit from this type.
/// </summary>
/// <remarks>
/// <para>
/// Provides the channel read loop, concurrency control, error handling, graceful shutdown,
/// and health reporting. Subclasses only need to implement <see cref="DispatchAsync"/> to
/// route events to the appropriate handlers.
/// </para>
/// <para>
/// Override <see cref="OnError"/> to customize error handling (e.g., retry, dead-letter).
/// The default implementation logs the error and marks the acknowledgement as failed.
/// </para>
/// </remarks>
/// <typeparam name="TEvent">The base event type for this queue.</typeparam>
public abstract class ChannelWorker<TEvent> : BackgroundService, IModule
{
    private readonly EventQueueChannel<TEvent> _channel;
    private readonly SemaphoreSlim? _throttle;
    private readonly ILogger _logger;
    private long _processedCount;
    private long _errorCount;
    private long _processingRateWindowStart;
    private long _processingRateWindowCount;

    /// <summary>
    /// Creates a new <see cref="ChannelWorker{TEvent}"/>.
    /// </summary>
    /// <param name="channel">The event queue channel to read from.</param>
    /// <param name="logger">Logger for diagnostics.</param>
    /// <param name="maxConcurrency">
    /// Maximum concurrent handlers. <c>1</c> = sequential, <c>0</c> = unlimited.
    /// </param>
    protected ChannelWorker(EventQueueChannel<TEvent> channel, ILogger logger, int maxConcurrency = 1)
    {
        _channel = channel;
        _logger = logger;
        _throttle = maxConcurrency > 1 ? new SemaphoreSlim(maxConcurrency, maxConcurrency) : null;
        _processingRateWindowStart = Environment.TickCount64;
    }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public ModuleHealth Health
    {
        get
        {
            if (_errorCount > 0 && _processedCount == 0) return ModuleHealth.Unhealthy;
            if (_errorCount > 0) return ModuleHealth.Degraded;
            return ModuleHealth.Healthy;
        }
    }

    /// <summary>
    /// Gets the number of items currently queued in the channel.
    /// Returns <c>-1</c> if the channel does not support counting.
    /// </summary>
    public int ChannelDepth => _channel.Count;

    /// <summary>
    /// Gets the total number of successfully processed events since the worker started.
    /// </summary>
    public long ProcessedCount => Interlocked.Read(ref _processedCount);

    /// <summary>
    /// Gets the total number of failed event processing attempts since the worker started.
    /// </summary>
    public long ErrorCount => Interlocked.Read(ref _errorCount);

    /// <summary>
    /// Gets the approximate processing rate in events per second over a sliding window.
    /// </summary>
    public double ProcessingRate
    {
        get
        {
            var elapsed = Environment.TickCount64 - Interlocked.Read(ref _processingRateWindowStart);
            if (elapsed <= 0) return 0;
            return Interlocked.Read(ref _processingRateWindowCount) / (elapsed / 1000.0);
        }
    }

    /// <summary>
    /// Dispatches an event to the appropriate handler(s). Implemented by generated subclasses.
    /// </summary>
    /// <param name="event">The event to dispatch.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    protected abstract Task DispatchAsync(TEvent @event, CancellationToken cancellationToken);

    /// <summary>
    /// Called when an event handler throws an exception. Override to implement retry, dead-letter,
    /// or custom error handling. The default implementation logs the error.
    /// </summary>
    /// <param name="event">The event that caused the error.</param>
    /// <param name="exception">The exception thrown by the handler.</param>
    protected virtual void OnError(TEvent @event, Exception exception)
    {
        _logger.LogError(exception, "[{QueueName}] Error processing event {EventType}",
            Name, @event?.GetType().Name ?? "unknown");
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[{QueueName}] Event queue worker started", Name);

        try
        {
            await foreach (var envelope in _channel.Reader.ReadAllAsync(stoppingToken).ConfigureAwait(false))
            {
                if (_throttle is not null)
                {
                    await _throttle.WaitAsync(stoppingToken).ConfigureAwait(false);
                    _ = ProcessWithThrottleAsync(envelope, stoppingToken);
                }
                else
                {
                    await ProcessAsync(envelope, stoppingToken).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Graceful shutdown
        }

        _logger.LogInformation("[{QueueName}] Event queue worker stopped. Processed: {Count}, Errors: {Errors}",
            Name, ProcessedCount, ErrorCount);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        _channel.Complete();
        _throttle?.Dispose();
        base.Dispose();
    }

    private async Task ProcessWithThrottleAsync(
        EventQueueChannel<TEvent>.Envelope envelope,
        CancellationToken cancellationToken)
    {
        try
        {
            await ProcessAsync(envelope, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _throttle!.Release();
        }
    }

    private async Task ProcessAsync(
        EventQueueChannel<TEvent>.Envelope envelope,
        CancellationToken cancellationToken)
    {
        try
        {
            await DispatchAsync(envelope.Event, cancellationToken).ConfigureAwait(false);
            Interlocked.Increment(ref _processedCount);
            Interlocked.Increment(ref _processingRateWindowCount);
            envelope.Acknowledgement?.MarkCompleted();
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _errorCount);
            OnError(envelope.Event, ex);
            envelope.Acknowledgement?.MarkFailed(ex);
        }

        ResetRateWindowIfNeeded();
    }

    private void ResetRateWindowIfNeeded()
    {
        var now = Environment.TickCount64;
        var windowStart = Interlocked.Read(ref _processingRateWindowStart);
        // Reset the sliding window every 10 seconds
        if (now - windowStart <= 10_000) return;
        Interlocked.Exchange(ref _processingRateWindowStart, now);
        Interlocked.Exchange(ref _processingRateWindowCount, 0);
    }
}
