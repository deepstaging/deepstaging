// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.EventQueue.Models;

/// <summary>
/// Model for an event queue discovered via <see cref="EventQueueAttribute"/>.
/// Contains all information needed to generate the worker, effect module, and DI registration.
/// </summary>
[PipelineModel]
public sealed record EventQueueModel
{
    /// <summary>
    /// The queue name (e.g., "DomainEvents").
    /// </summary>
    public required string QueueName { get; init; }

    /// <summary>
    /// The name of the partial class decorated with [EventQueue].
    /// </summary>
    public required string ContainerName { get; init; }

    /// <summary>
    /// The namespace of the queue class.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The accessibility modifier (e.g., "public", "internal").
    /// </summary>
    public required string Accessibility { get; init; }

    /// <summary>
    /// The fully qualified event base type, or <c>null</c> if unconstrained.
    /// </summary>
    public string? EventBaseType { get; init; }

    /// <summary>
    /// The event base type name without namespace, or <c>null</c> if unconstrained.
    /// </summary>
    public string? EventBaseTypeName { get; init; }

    /// <summary>
    /// Channel capacity. <c>0</c> means unbounded.
    /// </summary>
    public int Capacity { get; init; }

    /// <summary>
    /// Maximum concurrent handlers. <c>1</c> = sequential, <c>0</c> = unlimited.
    /// </summary>
    public int MaxConcurrency { get; init; } = 1;

    /// <summary>
    /// Timeout in milliseconds. <c>0</c> = no timeout.
    /// </summary>
    public int TimeoutMilliseconds { get; init; }

    /// <summary>
    /// Whether the channel uses a single reader.
    /// </summary>
    public bool SingleReader { get; init; } = true;

    /// <summary>
    /// Whether the channel uses a single writer.
    /// </summary>
    public bool SingleWriter { get; init; }

    /// <summary>
    /// The handler groups subscribed to this queue.
    /// </summary>
    public EquatableArray<EventQueueHandlerGroupModel> HandlerGroups { get; init; } = [];
}

/// <summary>
/// Model for a handler group (a class decorated with <c>[EventQueueHandler]</c>).
/// </summary>
[PipelineModel]
public sealed record EventQueueHandlerGroupModel
{
    /// <summary>
    /// The fully qualified name of the handler class.
    /// </summary>
    public required string HandlerType { get; init; }

    /// <summary>
    /// The handler class name without namespace.
    /// </summary>
    public required string HandlerTypeName { get; init; }

    /// <summary>
    /// The fully qualified runtime type this handler targets.
    /// </summary>
    public required string RuntimeType { get; init; }

    /// <summary>
    /// The individual handler methods in this group.
    /// </summary>
    public required EquatableArray<EventHandlerMethodModel> Methods { get; init; }
}

/// <summary>
/// Model for a single event handler method.
/// </summary>
[PipelineModel]
public sealed record EventHandlerMethodModel
{
    /// <summary>
    /// The method name (e.g., "OnOrderCreated").
    /// </summary>
    public required string MethodName { get; init; }

    /// <summary>
    /// The fully qualified event type this handler accepts.
    /// </summary>
    public required string EventType { get; init; }

    /// <summary>
    /// The event type name without namespace.
    /// </summary>
    public required string EventTypeName { get; init; }
}
