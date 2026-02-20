// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.EventQueue.Attributes;

/// <summary>
/// A queryable wrapper over <c>EventQueueHandlerAttribute&lt;TRuntime&gt;</c> data.
/// Provides access to the queue name and runtime type.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record EventQueueHandlerAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// Gets the queue name this handler subscribes to (first constructor argument).
    /// </summary>
    public string QueueName => ConstructorArg<string>(0)
        .OrThrow("EventQueueHandlerAttribute must have a queue name as its first constructor argument.");

    /// <summary>
    /// Gets the runtime type parameter from the generic attribute.
    /// </summary>
    public ValidSymbol<INamedTypeSymbol> RuntimeType =>
        AttributeData.AttributeClass!.TypeArguments[0]
            .AsValidNamedType();
}
