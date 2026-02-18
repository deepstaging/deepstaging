// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects;

using Models;

/// <summary>
/// Extension methods for querying runtime class information from symbols decorated with <see cref="RuntimeAttribute"/>.
/// </summary>
public static class RuntimeModelQueries
{
    extension(ValidSymbol<INamedTypeSymbol> runtime)
    {
        /// <summary>
        /// Builds a <see cref="Models.RuntimeModel"/> from a type decorated with <see cref="RuntimeAttribute"/>.
        /// Aggregates all capabilities from <see cref="UsesAttribute"/> declarations.
        /// </summary>
        /// <returns>A model containing the runtime configuration and all aggregated capabilities.</returns>
        public RuntimeModel QueryRuntimeModel()
        {
            var usesAttributes = runtime.UsesAttributes();

            var modules = usesAttributes
                .SelectMany(attr => attr.EffectsModules)
                .ToImmutableArray();

            var standaloneCapabilities = usesAttributes
                .SelectMany(attr => attr.Capabilities)
                .ToImmutableArray();

            return new RuntimeModel
            {
                RuntimeTypeName = runtime.Name,
                RuntimeType = runtime.FullyQualifiedName,
                Namespace = runtime.Namespace ?? "Global",
                AccessibilityModifier = runtime.AccessibilityString,
                Capabilities =
                [
                    ..modules.Select(model => model.Capability),
                    ..standaloneCapabilities
                ],
                ActivitySources =
                [
                    ..modules
                        .Where(model => model.Instrumented)
                        .Select(model => $"{model.Namespace}.{model.Name}")
                ],
                HasInstrumentedModules = modules.Any(model => model.Instrumented)
            };
        }
    }
}