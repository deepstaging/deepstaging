// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects;

using Attributes;

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

        /// <summary>
        /// Gets all <see cref="CapabilityAttribute"/> instances applied to this symbol as queryable wrappers.
        /// </summary>
        /// <returns>An immutable array of <see cref="CapabilityAttributeQuery"/> instances.</returns>
        public ImmutableArray<CapabilityAttributeQuery> CapabilityAttributes() =>
        [
            ..symbol.GetAttributes<CapabilityAttribute>()
                .Select(attr => attr.AsQuery<CapabilityAttributeQuery>())
        ];

        /// <summary>
        /// Gets the <see cref="RegistersWithAttribute"/> instance applied to this symbol, if any.
        /// </summary>
        public RegistersWithAttributeQuery? RegistersWithAttribute()
        {
            var attrs = symbol.GetAttributes<RegistersWithAttribute>();
            return attrs.Any()
                ? attrs.First().AsQuery<RegistersWithAttributeQuery>()
                : null;
        }
    }
}