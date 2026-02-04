// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Projection.Models;

namespace Deepstaging.Projection.Attributes;

/// <summary>
/// A queryable wrapper over <see cref="UsesAttribute"/> data.
/// Provides access to the referenced module type and its effects.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record UsesAttributeQuery(AttributeData AttributeData)
{
    /// <summary>
    /// Gets the module type referenced by this attribute.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the module type is not valid.</exception>
    public ValidSymbol<INamedTypeSymbol> ModuleType => AttributeData
        .GetConstructorArgument<INamedTypeSymbol>(0)
        .Map(symbol => symbol.AsValidNamedType())
        .OrThrow($"{nameof(UsesAttribute)} must have a valid module type as its first constructor argument.");

    /// <summary>
    /// Gets all effects modules defined on the referenced module type.
    /// </summary>
    public ImmutableArray<EffectsModuleModel> EffectsModules => ModuleType.QueryEffectsModules();
}