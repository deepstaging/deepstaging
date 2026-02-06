// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Projection.Attributes;

namespace Deepstaging.Projection;

/// <summary>
/// Extension methods for querying Deepstaging attributes on symbols.
/// </summary>
public static class Queries
{
    extension(ValidAttribute attribute)
    {
        /// <summary>
        /// Converts a <see cref="ValidAttribute"/> to a <see cref="UsesAttributeQuery"/>.
        /// </summary>
        public UsesAttributeQuery QueryUsesAttribute() =>
            attribute.AsQuery<UsesAttributeQuery>();

        /// <summary>
        /// Converts a <see cref="ValidAttribute"/> to an <see cref="EffectsModuleAttributeQuery"/>.
        /// </summary>
        public EffectsModuleAttributeQuery QueryEffectsModuleAttribute() =>
            attribute.AsQuery<EffectsModuleAttributeQuery>();
    }

    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Gets all <see cref="EffectsModuleAttribute"/> instances applied to this symbol as queryable wrappers.
        /// </summary>
        /// <returns>An immutable array of <see cref="EffectsModuleAttributeQuery"/> instances.</returns>
        public ImmutableArray<EffectsModuleAttributeQuery> EffectsModuleAttributes() =>
        [
            ..symbol.GetAttributes<EffectsModuleAttribute>()
                .Select(attr => attr.AsQuery<EffectsModuleAttributeQuery>())
        ];
        
        /// <summary>
        /// Gets all <see cref="UsesAttribute"/> instances applied to this symbol as queryable wrappers.
        /// </summary>
        /// <returns>An immutable array of <see cref="UsesAttributeQuery"/> instances.</returns>
        public ImmutableArray<UsesAttributeQuery> UsesAttributes() =>
        [
            ..symbol.GetAttributes<UsesAttribute>()
                .Select(attr => attr.AsQuery<UsesAttributeQuery>())
        ];
    }
}