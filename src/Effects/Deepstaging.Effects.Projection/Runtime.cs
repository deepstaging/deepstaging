// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Effects.Projection.Models;

namespace Deepstaging.Effects.Projection;

/// <summary>
/// Extension methods for querying runtime class information from symbols decorated with <see cref="RuntimeAttribute"/>.
/// </summary>
public static class Runtime
{
    extension(ValidSymbol<INamedTypeSymbol> runtime)
    {
        /// <summary>
        /// Builds a <see cref="RuntimeModel"/> from a type decorated with <see cref="RuntimeAttribute"/>.
        /// Aggregates all capabilities from <see cref="UsesAttribute"/> declarations.
        /// </summary>
        /// <returns>A model containing the runtime configuration and all aggregated capabilities.</returns>
        public RuntimeModel QueryRuntimeModel()
        {
            return new RuntimeModel
            {
                RuntimeTypeName = runtime.Name,
                RuntimeType = runtime.FullyQualifiedName,
                Namespace = runtime.Namespace ?? "Global",
                AccessibilityModifier = runtime.AccessibilityString,
                Capabilities =
                [
                    ..runtime.UsesAttributes()
                        .SelectMany(attr => attr.EffectsModules)
                        .Select(model => model.Capability)
                ]
            };
        }
    }
}