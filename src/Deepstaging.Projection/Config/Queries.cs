// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Config;

using Attributes;

/// <summary>
/// Extension methods for querying Config attributes on symbols.
/// </summary>
public static class Queries
{
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Gets the <see cref="ConfigProviderAttribute"/> applied to this symbol as a queryable wrapper.
        /// </summary>
        public ConfigProviderAttributeQuery ConfigProviderAttribute() =>
            symbol.GetAttribute<ConfigProviderAttribute>()
                .Map(attr => attr.AsQuery<ConfigProviderAttributeQuery>())
                .OrThrow("ConfigProviderAttribute is required on ConfigProvider classes.");

        /// <summary>
        /// Gets all <see cref="ExposesAttribute{T}"/> instances applied to this symbol as queryable wrappers.
        /// </summary>
        /// <returns>An immutable array of <see cref="ExposesAttributeQuery"/> instances.</returns>
        public ImmutableArray<ExposesAttributeQuery> ExposesAttributes() =>
        [
            ..symbol.GetAttributes(typeof(ExposesAttribute<>))
                .Select(x => x.AsQuery<ExposesAttributeQuery>())
        ];
    }
}