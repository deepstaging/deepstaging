// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Effects.Projection.Models;

/// <summary>
/// Model for a standalone effects module discovered via [EffectsModule] attribute.
/// </summary>
public sealed record EffectsModuleModel
{
    /// <summary>
    /// The name of the module (e.g., "Email", "Database").
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The name of the partial class decorated with [EffectsModule].
    /// </summary>
    public required string EffectsContainerName { get; init; }

    /// <summary>
    /// The namespace of the module class.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The accessibility modifier (e.g., "public", "internal").
    /// </summary>
    public required string Accessibility { get; init; }

    /// <summary>
    /// The fully qualified target type.
    /// </summary>
    public required string TargetType { get; init; }

    /// <summary>
    /// The target type name without namespace.
    /// </summary>
    public required string TargetTypeName { get; init; }

    /// <summary>
    /// The capability interface and property information for this module.
    /// </summary>
    public required RuntimeCapabilityModel Capability { get; init; }

    /// <summary>
    /// The effect methods in this module.
    /// </summary>
    public required ImmutableArray<EffectMethodModel> Methods { get; init; }

    /// <summary>
    /// Whether OpenTelemetry instrumentation is enabled.
    /// </summary>
    public bool Instrumented { get; init; } = true;

    /// <summary>
    /// Whether this is a DbContext module.
    /// </summary>
    public bool IsDbContext { get; init; }

    /// <summary>
    /// DbSet information if this is a DbContext module.
    /// </summary>
    public ImmutableArray<DbSetModel> DbSets { get; init; } = [];

    /// <summary>
    /// XML documentation from the target type (if available).
    /// </summary>
    public XmlDocumentation XmlDocumentation { get; init; }
}

/// <summary>
/// Model for a DbSet within a DbContext module.
/// </summary>
public sealed record DbSetModel
{
    /// <summary>
    /// The property name (e.g., "Users").
    /// </summary>
    public required string PropertyName { get; init; }

    /// <summary>
    /// The entity type (fully qualified).
    /// </summary>
    public required string EntityType { get; init; }

    /// <summary>
    /// The entity type name without namespace.
    /// </summary>
    public required string EntityTypeName { get; init; }
}
