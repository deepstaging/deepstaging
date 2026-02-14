// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;

namespace Deepstaging.Ids.Projection;

/// <summary>
/// Represents the analyzed model of a strongly-typed ID struct.
/// </summary>
[PipelineModel]
public sealed record StrongIdModel
{
    /// <summary>
    /// The namespace containing the struct.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The name of the struct.
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// The accessibility of the struct (public, internal, etc.).
    /// </summary>
    public required string Accessibility { get; init; }

    /// <summary>
    /// The backing type for the ID value.
    /// </summary>
    public required BackingType BackingType { get; init; }

    /// <summary>
    /// The converters to generate.
    /// </summary>
    public required IdConverters Converters { get; init; }

    /// <summary>
    /// Pipeline-safe snapshot of the backing type (Guid, Int32, Int64, or String).
    /// </summary>
    public required TypeSnapshot BackingTypeSnapshot { get; init; }
}
