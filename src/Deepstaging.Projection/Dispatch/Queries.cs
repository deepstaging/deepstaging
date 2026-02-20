// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Dispatch;

using Attributes;
using Deepstaging.Dispatch;

/// <summary>
/// Extension methods for querying Dispatch attributes on symbols.
/// </summary>
public static class Queries
{
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Gets all <see cref="DispatchModuleAttribute"/> instances applied to this symbol as queryable wrappers.
        /// </summary>
        public ImmutableArray<DispatchModuleAttributeQuery> DispatchModuleAttributes() =>
        [
            ..symbol.GetAttributes<DispatchModuleAttribute>()
                .Select(attr => attr.AsQuery<DispatchModuleAttributeQuery>())
        ];
    }
}
