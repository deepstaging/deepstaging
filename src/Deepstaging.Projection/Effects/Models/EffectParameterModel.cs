// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects.Models;

/// <summary>
/// Represents a parameter for an effect method.
/// </summary>
[PipelineModel]
public sealed record EffectParameterModel
{
    /// <summary>
    /// The fully qualified type of the parameter.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// The parameter name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Whether this parameter has a default value.
    /// </summary>
    public bool HasDefaultValue { get; init; }

    /// <summary>
    /// The default value expression (if HasDefaultValue is true).
    /// </summary>
    public string? DefaultValue { get; init; }
}