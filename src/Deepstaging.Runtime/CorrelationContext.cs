// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging;

/// <summary>
/// Carries cross-cutting metadata through the request pipeline — from HTTP endpoints
/// through Dispatch handlers and into EventQueue processing.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="CorrelationContext"/> flows through two mechanisms:
/// <list type="bullet">
///   <item>
///     <description>
///     <b>Explicit:</b> runtimes implementing <see cref="Effects.IHasCorrelationContext"/>
///     expose it as a typed property.
///     </description>
///   </item>
///   <item>
///     <description>
///     <b>Ambient:</b> <see cref="Current"/> uses <see cref="AsyncLocal{T}"/> for
///     infrastructure code (instrumentation, EventQueue envelope propagation).
///     </description>
///   </item>
/// </list>
/// </para>
/// <para>
/// EventQueue captures the ambient context at enqueue time and restores it
/// before dispatching handlers, preserving correlation across async boundaries.
/// </para>
/// </remarks>
/// <param name="CorrelationId">
/// A unique identifier for the entire distributed operation (e.g., HTTP request ID).
/// </param>
/// <param name="UserId">
/// The authenticated user who initiated the operation, if any.
/// </param>
/// <param name="TenantId">
/// The tenant scope for multi-tenant applications, if any.
/// </param>
/// <param name="CausationId">
/// Identifies the command, query, or event that directly caused the current operation.
/// Set automatically by infrastructure when events flow through EventQueue.
/// </param>
/// <param name="Metadata">
/// Arbitrary key-value pairs for application-specific context (feature flags, API version, etc.).
/// </param>
public sealed record CorrelationContext(
    string? CorrelationId = null,
    string? UserId = null,
    string? TenantId = null,
    string? CausationId = null,
    IReadOnlyDictionary<string, string>? Metadata = null)
{
    private static readonly AsyncLocal<CorrelationContext?> Ambient = new();

    /// <summary>
    /// Gets or sets the ambient <see cref="CorrelationContext"/> for the current async flow.
    /// </summary>
    /// <remarks>
    /// Set by infrastructure (e.g., generated endpoint middleware, EventQueue worker).
    /// Read by <see cref="Effects.ActivityEffectExtensions.WithActivity{RT,A}"/> when
    /// the runtime does not implement <see cref="Effects.IHasCorrelationContext"/>.
    /// </remarks>
    public static CorrelationContext? Current
    {
        get => Ambient.Value;
        set => Ambient.Value = value;
    }
}
