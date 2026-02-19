// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects.Models;

/// <summary>
/// Model for a standalone capability declared via <see cref="CapabilityAttribute"/>.
/// Contains the capability information and the namespace of the declaring container class.
/// </summary>
[PipelineModel]
public sealed record CapabilityModel
{
    /// <summary>
    /// The namespace of the declaring container class.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The capability to generate an <c>IHas*</c> interface for.
    /// </summary>
    public required RuntimeCapabilityModel Capability { get; init; }
}
