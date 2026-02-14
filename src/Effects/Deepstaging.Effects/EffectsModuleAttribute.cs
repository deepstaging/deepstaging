// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging;

/// <summary>
/// Defines a standalone effects module that wraps methods from a target type into LanguageExt effects.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class EffectsModuleAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of the module.
    /// If not set, the name is derived from the target type (e.g., "EmailService" for IEmailService).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the target type whose methods will be wrapped as effects.
    /// Can be an interface (methods wrapped) or a <c>DbContext</c> (entity query builders generated).
    /// </summary>
    public Type TargetType { get; }

    /// <summary>
    /// Gets or sets whether OpenTelemetry instrumentation is enabled.
    /// When <c>true</c> (default), generated effects include <c>.WithActivity()</c> calls
    /// that create spans for tracing. Zero overhead when no <c>ActivityListener</c> is registered.
    /// </summary>
    public bool Instrumented { get; init; } = true;

    /// <summary>
    /// When set, ONLY these methods are wrapped as effects.
    /// All other methods on the target type are ignored.
    /// Takes precedence over <see cref="Exclude"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // Only wrap SendAsync and SendBulkAsync
    /// [EffectsModule(typeof(IEmailService), IncludeOnly = ["SendAsync", "SendBulkAsync"])]
    /// public partial class EmailModule;
    /// </code>
    /// </example>
    public string[]? IncludeOnly { get; init; }

    /// <summary>
    /// Methods to exclude from effect generation.
    /// Ignored if <see cref="IncludeOnly"/> is set.
    /// </summary>
    /// <example>
    /// <code>
    /// // Exclude internal/diagnostic methods
    /// [EffectsModule(typeof(IEmailService), Exclude = ["GetStatistics", "Ping"])]
    /// public partial class EmailModule;
    /// </code>
    /// </example>
    public string[]? Exclude { get; init; }

    /// <summary>
    /// Creates a new EffectsModule attribute.
    /// </summary>
    /// <param name="targetType">The type to wrap. Can be an interface or <c>DbContext</c>.</param>
    public EffectsModuleAttribute(Type targetType) =>
        TargetType = targetType;
}
