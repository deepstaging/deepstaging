// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects.Models;

/// <summary>
/// Model representing a <see cref="RegistersWithAttribute"/> declaration on a module.
/// Captures the DI registration method signature for bootstrapper aggregation.
/// </summary>
[PipelineModel]
public sealed record RegistersWithModel
{
    /// <summary>
    /// The fully qualified type name of the containing class.
    /// </summary>
    public required string ContainingType { get; init; }

    /// <summary>
    /// The name of the static extension method.
    /// </summary>
    public required string MethodName { get; init; }

    /// <summary>
    /// The fully qualified return type of the method.
    /// </summary>
    public required string ReturnType { get; init; }

    /// <summary>
    /// Additional parameters beyond the first (this IServiceCollection) parameter.
    /// These get mirrored onto the generated bootstrapper method.
    /// </summary>
    public EquatableArray<RegistersWithParameterModel> AdditionalParameters { get; init; } = [];
}

/// <summary>
/// Model for an additional parameter on a RegistersWith method.
/// </summary>
[PipelineModel]
public sealed record RegistersWithParameterModel
{
    /// <summary>The parameter name.</summary>
    public required string Name { get; init; }

    /// <summary>The fully qualified parameter type.</summary>
    public required string Type { get; init; }

    /// <summary>Whether the parameter has a default value.</summary>
    public bool HasDefaultValue { get; init; }

    /// <summary>The default value expression, if any.</summary>
    public string? DefaultValue { get; init; }
}
