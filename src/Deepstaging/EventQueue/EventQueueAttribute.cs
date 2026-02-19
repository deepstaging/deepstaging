// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.EventQueue;

/// <summary>
/// Marks a static partial class as an in-process event queue backed by a <c>System.Threading.Channels.Channel&lt;T&gt;</c>.
/// </summary>
/// <remarks>
/// <para>
/// The generator produces a <c>BackgroundService</c> that reads from the channel and dispatches
/// to subscriber methods, an effect module with <c>Enqueue</c>/<c>EnqueueWithAck</c>/<c>EnqueueAndWait</c>
/// methods, and DI registration extensions.
/// </para>
/// <para>
/// The generated module implements <c>IEffectModule</c> and can be composed into a Runtime
/// via <c>[Uses(typeof(MyQueue))]</c>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [EventQueue("DomainEvents", EventBaseType = typeof(IDomainEvent))]
/// public static partial class DomainEvents;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EventQueueAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the event queue. Used for DI registration and subscriber matching.
    /// </summary>
    public string QueueName { get; }

    /// <summary>
    /// Gets or sets the base type constraint for events published to this queue.
    /// When set, only events assignable to this type can be enqueued.
    /// </summary>
    public Type? EventBaseType { get; init; }

    /// <summary>
    /// Gets or sets the channel capacity. A value of <c>0</c> (default) creates an unbounded channel.
    /// </summary>
    public int Capacity { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of concurrent event handlers.
    /// <c>1</c> (default) processes events sequentially. <c>0</c> allows unlimited concurrency.
    /// </summary>
    public int MaxConcurrency { get; init; } = 1;

    /// <summary>
    /// Gets or sets the timeout in milliseconds for event processing.
    /// <c>0</c> (default) means no timeout.
    /// </summary>
    public int TimeoutMilliseconds { get; init; }

    /// <summary>
    /// Gets or sets whether the channel uses a single reader. Enables optimizations when <c>true</c>.
    /// Default is <c>true</c>.
    /// </summary>
    public bool SingleReader { get; init; } = true;

    /// <summary>
    /// Gets or sets whether the channel uses a single writer.
    /// Default is <c>false</c>.
    /// </summary>
    public bool SingleWriter { get; init; }

    /// <summary>
    /// Creates a new <see cref="EventQueueAttribute"/>.
    /// </summary>
    /// <param name="queueName">The name of the event queue.</param>
    public EventQueueAttribute(string queueName)
    {
        QueueName = queueName;
    }
}
