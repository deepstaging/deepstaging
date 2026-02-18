// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects;

using Models;

/// <summary>
/// Extension methods for building capability models from symbols decorated with <see cref="CapabilityAttribute"/>.
/// </summary>
public static class CapabilityModelQueries
{
    extension(ValidSymbol<INamedTypeSymbol> container)
    {
        /// <summary>
        /// Queries all <see cref="CapabilityAttribute"/> instances on this type and builds corresponding models.
        /// </summary>
        /// <returns>An immutable array of <see cref="CapabilityModel"/> instances, one per attribute.</returns>
        public ImmutableArray<CapabilityModel> QueryCapabilities()
        {
            return
            [
                ..container
                    .CapabilityAttributes()
                    .Where(attr => attr.HasValidTargetType)
                    .Select(attr => new CapabilityModel
                    {
                        Namespace = container.Namespace ?? "Global",
                        Capability = attr.TargetType.CreateCapabilityModel()
                    })
            ];
        }
    }
}
