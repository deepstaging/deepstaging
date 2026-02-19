// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.EventQueue;

/// <summary>
/// Marks a static class as containing event handler methods for a named event queue.
/// </summary>
/// <remarks>
/// <para>
/// Handler methods must be <c>static</c>, return <c>Eff&lt;TRuntime, Unit&gt;</c>,
/// and accept exactly one parameter (the event type).
/// </para>
/// <para>
/// The <typeparamref name="TRuntime"/> type parameter specifies which Runtime this handler
/// targets, allowing the generator to resolve the correct effect capabilities.
/// </para>
/// </remarks>
/// <typeparam name="TRuntime">The Runtime type this handler targets.</typeparam>
/// <example>
/// <code>
/// [EventQueueHandler&lt;AppRuntime&gt;("DomainEvents")]
/// public static class OrderEventHandlers
/// {
///     public static Eff&lt;AppRuntime, Unit&gt; OnOrderCreated(OrderCreated evt) =&gt; ...
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EventQueueHandlerAttribute<TRuntime> : Attribute
{
    /// <summary>
    /// Gets the name of the event queue this handler subscribes to.
    /// </summary>
    public string QueueName { get; }

    /// <summary>
    /// Creates a new <see cref="EventQueueHandlerAttribute{TRuntime}"/>.
    /// </summary>
    /// <param name="queueName">The name of the event queue to handle events from.</param>
    public EventQueueHandlerAttribute(string queueName)
    {
        QueueName = queueName;
    }
}
