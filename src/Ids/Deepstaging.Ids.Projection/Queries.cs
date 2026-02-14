// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;

namespace Deepstaging.Ids.Projection;

/// <summary>
/// Extension methods for querying strongly-typed ID attributes from symbols.
/// </summary>
public static class Queries
{
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Creates a <see cref="StrongIdModel"/> from the symbol.
        /// </summary>
        public StrongIdModel ToStrongIdModel(SemanticModel model)
        {
            return symbol.GetAttribute<StrongIdAttribute>()
                .Map(attr => attr.AsQuery<StrongIdAttributeQuery>())
                .Map(attr => new StrongIdModel
                {
                    Namespace = symbol.Namespace ?? "",
                    TypeName = symbol.Name,
                    Accessibility = symbol.AccessibilityString,
                    BackingType = attr.BackingType,
                    BackingTypeSymbol = attr.BackingTypeSymbol(model),
                    Converters = attr.Converters
                })
                .OrThrow($"Expected symbol '{symbol.FullyQualifiedName}' to have StrongIdAttribute.");
        }
    }
}
