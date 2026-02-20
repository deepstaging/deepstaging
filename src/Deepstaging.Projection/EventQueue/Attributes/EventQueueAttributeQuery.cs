// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.EventQueue.Attributes;

using Attr = EventQueueAttribute;

/// <summary>
/// A queryable wrapper over <see cref="EventQueueAttribute"/> data.
/// Provides strongly-typed access to attribute properties with sensible defaults.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record EventQueueAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// Gets the queue name (first constructor argument).
    /// </summary>
    public string QueueName => ConstructorArg<string>(0)
        .OrThrow($"{nameof(Attr)} must have a queue name as its first constructor argument.");

    /// <summary>
    /// Gets the event base type constraint, if specified.
    /// Returns <c>null</c> when no constraint is set.
    /// </summary>
    public ValidSymbol<INamedTypeSymbol>? EventBaseType => NamedArg<INamedTypeSymbol>(nameof(Attr.EventBaseType))
        .Map(symbol => symbol.AsValidNamedType())
        .OrNull();

    /// <summary>
    /// Gets whether the event base type is specified and valid.
    /// </summary>
    public bool HasEventBaseType => NamedArg<INamedTypeSymbol>(nameof(Attr.EventBaseType))
        .Map(symbol => symbol.AsNamedType())
        .Map(symbol => symbol.IsValid(out _))
        .OrDefault(false);

    /// <summary>
    /// Gets the channel capacity. <c>0</c> means unbounded.
    /// </summary>
    public int Capacity => NamedArg<int>(nameof(Attr.Capacity))
        .OrDefault(0);

    /// <summary>
    /// Gets the maximum concurrency. <c>1</c> means sequential processing.
    /// </summary>
    public int MaxConcurrency => NamedArg<int>(nameof(Attr.MaxConcurrency))
        .OrDefault(1);

    /// <summary>
    /// Gets the timeout in milliseconds. <c>0</c> means no timeout.
    /// </summary>
    public int TimeoutMilliseconds => NamedArg<int>(nameof(Attr.TimeoutMilliseconds))
        .OrDefault(0);

    /// <summary>
    /// Gets whether the channel uses a single reader.
    /// </summary>
    public bool SingleReader => NamedArg<bool>(nameof(Attr.SingleReader))
        .OrDefault(true);

    /// <summary>
    /// Gets whether the channel uses a single writer.
    /// </summary>
    public bool SingleWriter => NamedArg<bool>(nameof(Attr.SingleWriter))
        .OrDefault(false);
}
