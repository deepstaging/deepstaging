// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Config;

using ConfigModel = Projection.Config.Models.ConfigModel;

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
            var @interface = TypeBuilder
                .Interface($"I{model.TypeName}")
                .InNamespace(model.Namespace);

            var type = TypeBuilder
                .Class(model.TypeName)
                .AsPartial()
                .WithAccessibility(model.Accessibility)
                .Implements(@interface.Name)
                .InNamespace(model.Namespace)
                .WithEach(model.ExposedConfigurationTypes, AddConfigType)
                .Emit();

            return type.Combine(@interface.Emit());
        }
    }

    private static TypeBuilder AddConfigType(TypeBuilder builder, ConfigTypeModel configTypeModel) =>
        builder;
}