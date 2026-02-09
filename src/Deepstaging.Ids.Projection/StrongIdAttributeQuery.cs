// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;

namespace Deepstaging.Ids.Projection;

/// <summary>
/// Query wrapper for <see cref="StrongIdAttribute"/> data.
/// </summary>
public sealed record StrongIdAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// Gets the backing type for the strongly-typed ID.
    /// </summary>
    public BackingType BackingType =>
        NamedArg<int>(nameof(StrongIdAttribute.BackingType))
            .ToEnum<BackingType>()
            .OrDefault(BackingType.Guid);

    /// <summary>
    /// Gets the Roslyn symbol for the backing type.
    /// </summary>
    /// <param name="model">The semantic model to resolve well-known types from.</param>
    /// <returns>The symbol for the backing type (Guid, Int32, Int64, or String).</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="BackingType"/> has an unexpected value.</exception>
    public ValidSymbol<INamedTypeSymbol> BackingTypeSymbol(SemanticModel model)
    {
        return BackingType switch
        {
            BackingType.Guid => model.WellKnownSymbols.Guid,
            BackingType.Int => model.WellKnownSymbols.Int32,
            BackingType.Long => model.WellKnownSymbols.Int64,
            BackingType.String => model.WellKnownSymbols.String,
            _ => throw new ArgumentOutOfRangeException(nameof(BackingType), BackingType, null)
        };
    }

    /// <summary>
    /// Gets the converters to generate for the strongly-typed ID.
    /// </summary>
    public IdConverters Converters =>
        NamedArg<int>(nameof(StrongIdAttribute.Converters))
            .ToEnum<IdConverters>()
            .OrDefault(IdConverters.None);
}