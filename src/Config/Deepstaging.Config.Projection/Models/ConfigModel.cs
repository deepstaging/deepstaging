// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Config.Projection.Models;

/// <summary>
/// 
/// </summary>
public sealed record ConfigModel
{
    /// <summary>
    /// The namespace containing the type.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The simple name of the type.
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required Accessibility Accessibility { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public ImmutableArray<ValidSymbol<INamedTypeSymbol>> ExposedConfigurationTypes { get; init; } = [];
}
