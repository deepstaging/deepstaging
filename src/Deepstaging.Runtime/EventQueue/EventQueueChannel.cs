// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Threading.Channels;

namespace Deepstaging.EventQueue;

/// <summary>
/// A typed channel wrapper that provides <c>Enqueue</c>, <c>EnqueueWithAck</c>, and <c>EnqueueAndWait</c>
/// operations for an event queue. Generated effect modules delegate to this type.
/// </summary>
/// <typeparam name="TEvent">The base event type for this queue.</typeparam>
public sealed class EventQueueChannel<TEvent>
{
    private readonly Channel<Envelope> _channel;

    /// <summary>
    /// Creates an unbounded event queue channel.
    /// </summary>
    /// <param name="singleReader">Whether the channel has a single reader. Default is <c>true</c>.</param>
    /// <param name="singleWriter">Whether the channel has a single writer. Default is <c>false</c>.</param>
    public EventQueueChannel(bool singleReader = true, bool singleWriter = false)
    {
        _channel = Channel.CreateUnbounded<Envelope>(new UnboundedChannelOptions
        {
            SingleReader = singleReader,
            SingleWriter = singleWriter
        });
    }

    /// <summary>
    /// Creates a bounded event queue channel.
    /// </summary>
    /// <param name="capacity">The maximum number of items the channel can hold.</param>
    /// <param name="singleReader">Whether the channel has a single reader. Default is <c>true</c>.</param>
    /// <param name="singleWriter">Whether the channel has a single writer. Default is <c>false</c>.</param>
    public EventQueueChannel(int capacity, bool singleReader = true, bool singleWriter = false)
    {
        _channel = Channel.CreateBounded<Envelope>(new BoundedChannelOptions(capacity)
        {
            SingleReader = singleReader,
            SingleWriter = singleWriter,
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    /// <summary>
    /// Gets the channel reader for consuming events.
    /// </summary>
    internal ChannelReader<Envelope> Reader => _channel.Reader;

    /// <summary>
    /// Gets the number of items currently in the channel.
    /// </summary>
    public int Count => _channel.Reader.CanCount ? _channel.Reader.Count : -1;

    /// <summary>
    /// Marks the channel as complete, preventing further writes.
    /// </summary>
    internal void Complete() => _channel.Writer.TryComplete();

    /// <summary>
    /// Enqueues an event for fire-and-forget processing.
    /// </summary>
    /// <param name="event">The event to enqueue.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async ValueTask EnqueueAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(new Envelope(@event, null), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Enqueues an event and returns an acknowledgement token that completes when all handlers finish.
    /// </summary>
    /// <param name="event">The event to enqueue.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="EventAcknowledgement"/> that tracks handler completion.</returns>
    public async ValueTask<EventAcknowledgement> EnqueueWithAckAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        var ack = new EventAcknowledgement();
        await _channel.Writer.WriteAsync(new Envelope(@event, ack), cancellationToken).ConfigureAwait(false);
        return ack;
    }

    /// <summary>
    /// Enqueues an event and waits for all handlers to complete.
    /// </summary>
    /// <param name="event">The event to enqueue.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task EnqueueAndWaitAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        var ack = await EnqueueWithAckAsync(@event, cancellationToken).ConfigureAwait(false);
        await ack.Completion.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Wraps an event with an optional acknowledgement for channel transport.
    /// </summary>
    internal readonly record struct Envelope(TEvent Event, EventAcknowledgement? Acknowledgement);
}
