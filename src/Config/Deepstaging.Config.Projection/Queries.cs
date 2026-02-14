// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Config.Projection.Attributes;

namespace Deepstaging.Config.Projection;

/// <summary>
/// Extension methods for querying Config attributes on symbols.
/// </summary>
public static class Queries
{
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
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
