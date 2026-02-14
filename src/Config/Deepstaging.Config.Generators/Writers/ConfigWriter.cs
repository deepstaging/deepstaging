// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;
using Deepstaging.Config.Projection.Models;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;

namespace Deepstaging.Config.Generators.Writers;

/// <summary>
/// </summary>
public static class ConfigWriter
{
    extension(ConfigModel model)
    {
        /// <summary>
        /// </summary>
        public OptionalEmit WriteConfigClass()
        {
            var iface = TypeBuilder.Interface($"I{model.TypeName}")
                .InNamespace(model.Namespace)
                .Emit();

            var type = TypeBuilder.Parse($"partial class {model.TypeName} : I{model.TypeName}")
                .WithAccessibility(model.Accessibility)
                .InNamespace(model.Namespace)
                .WithEach(model.ExposedConfigurationTypes, AddConfigType).Emit();

            return OptionalEmit.Combine(type, iface);
        }
    }

    private static TypeBuilder AddConfigType(TypeBuilder builder, ValidSymbol<INamedTypeSymbol> configType) =>
        builder;
}
