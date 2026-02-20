// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.EventQueue;

using Attributes;
using Deepstaging.EventQueue;

/// <summary>
/// Extension methods for querying EventQueue attributes on symbols.
/// </summary>
public static class Queries
{
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Gets all <see cref="EventQueueAttribute"/> instances applied to this symbol as queryable wrappers.
        /// </summary>
        public ImmutableArray<EventQueueAttributeQuery> EventQueueAttributes() =>
        [
            ..symbol.GetAttributes<EventQueueAttribute>()
                .Select(attr => attr.AsQuery<EventQueueAttributeQuery>())
        ];
    }
}
