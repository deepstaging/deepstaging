// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Ids;

using Attributes;
using Models;

/// <summary>
/// Extension methods for querying strongly-typed ID attributes from symbols.
/// </summary>
public static class Queries
{
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Creates a <see cref="TypedIdModel"/> from the symbol.
        /// </summary>
        public TypedIdModel ToTypedIdModel(SemanticModel model)
        {
            return symbol.GetAttribute<TypedIdAttribute>()
                .Map(attr => attr.AsQuery<TypedIdAttributeQuery>())
                .Map(attr => new TypedIdModel
                    {
                        Namespace = symbol.Namespace ?? "",
                        TypeName = symbol.Name,
                        Accessibility = symbol.AccessibilityString,
                        BackingType = attr.BackingType,
                        BackingTypeSnapshot = attr.BackingTypeSymbol(model).ToSnapshot(),
                        Converters = attr.Converters
                    }
                )
                .OrThrow($"Expected symbol '{symbol.FullyQualifiedName}' to have TypedIdAttribute.");
        }
    }
}