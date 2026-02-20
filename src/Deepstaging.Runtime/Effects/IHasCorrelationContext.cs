// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects;

/// <summary>
/// Runtime capability interface for providing a <see cref="CorrelationContext"/> to effect
/// instrumentation and event processing.
/// </summary>
/// <remarks>
/// <para>
/// When the runtime implements this interface,
/// <see cref="ActivityEffectExtensions.WithActivity{RT,A}"/> enriches activity spans
/// with correlation tags (<c>correlation.id</c>, <c>user.id</c>, <c>tenant.id</c>,
/// <c>causation.id</c>) and wraps logging in a structured scope containing all context fields.
/// </para>
/// <para>
/// If the runtime does not implement this interface, instrumentation falls back to
/// <see cref="CorrelationContext.Current"/> (ambient <see cref="AsyncLocal{T}"/>).
/// </para>
/// </remarks>
public interface IHasCorrelationContext
{
    /// <summary>
    /// The correlation context for the current operation.
    /// </summary>
    CorrelationContext? CorrelationContext { get; }
}
