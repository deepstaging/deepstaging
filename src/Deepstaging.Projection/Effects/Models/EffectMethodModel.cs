// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects.Models;

/// <summary>
/// Represents a single effect method derived from an interface method.
/// </summary>
[PipelineModel]
public sealed record EffectMethodModel
{
    /// <summary>
    /// The name of the effect method (may differ from source method name).
    /// </summary>
    public required string EffectName { get; init; }

    /// <summary>
    /// The original method name on the interface.
    /// </summary>
    public required string SourceMethodName { get; init; }

    /// <summary>
    /// The result type for the Eff (e.g., "Unit", "string", "Option&lt;User&gt;").
    /// </summary>
    public required string EffResultType { get; init; }

    /// <summary>
    /// The lifting strategy to use for this method.
    /// </summary>
    public required EffectLiftingStrategy LiftingStrategy { get; init; }

    /// <summary>
    /// The parameters for this method.
    /// </summary>
    public required EquatableArray<EffectParameterModel> Parameters { get; init; }

    /// <summary>
    /// XML documentation from the source method (if available).
    /// </summary>
    public DocumentationSnapshot Documentation { get; init; } = DocumentationSnapshot.Empty;
}