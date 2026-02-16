// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Testing.Projection;

using Models;

/// <summary>
/// Extension methods for querying test runtime attributes on symbols.
/// </summary>
public static class Queries
{
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Queries the <see cref="TestRuntimeAttribute{TRuntime}"/> on this type and builds a
        /// <see cref="TestRuntimeModel"/> by discovering capabilities from the production runtime.
        /// </summary>
        /// <returns>A model containing the test runtime configuration.</returns>
        public TestRuntimeModel QueryTestRuntimeModel()
        {
            var attribute = symbol
                .GetAttribute(typeof(TestRuntimeAttribute<>))
                .Map(attr => attr.AsQuery<TestRuntimeAttributeQuery>())
                .OrThrow("Expected TestRuntimeAttribute on symbol.");

            return new TestRuntimeModel
            {
                TestRuntimeType = symbol.ToSnapshot(),
                Namespace = symbol.Namespace ?? throw new InvalidOperationException("Symbol must have a namespace."),
                RuntimeType = attribute.RuntimeType.ToSnapshot(),
                AccessibilityModifier = symbol.AccessibilityString,
                Capabilities =
                [
                    // Discover capabilities by querying the production runtime's [Uses] modules
                    ..attribute.RuntimeType.UsesAttributes()
                        .SelectMany(attr => attr.EffectsModules)
                        .Select(model => model.Capability)
                ]
            };
        }
    }
}
