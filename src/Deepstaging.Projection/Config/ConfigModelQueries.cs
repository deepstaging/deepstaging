// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Config;

using Attributes;
using Models;

/// <summary>
/// Provides extension methods for querying and building "With" method models from type symbols.
/// </summary>
public static class ConfigModelQueries
{
    /// <summary>
    /// </summary>
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Builds a <see cref="Models.ConfigModel"/> from the current symbol, extracting relevant information such as namespace, type name, accessibility, and exposed configuration types.
        /// </summary>
        /// <returns></returns>
        public ConfigModel QueryConfigModel() =>
            new()
            {
                Namespace = symbol.Namespace ?? "",
                TypeName = symbol.Name,
                Accessibility = symbol.AccessibilityString,
                ExposedConfigurationTypes = symbol.ExposesAttributes()
                    .Select(ToConfigTypeModel)
                    .ToEquatableArray()
            };
    }

    private static ConfigTypeModel ToConfigTypeModel(ExposesAttributeQuery attribute)
    {
        var properties = attribute.ConfigurationType
            .QueryProperties()
            .ThatAreInstance()
            .Select(ToConfigTypePropertyModel);

        return new ConfigTypeModel(Type: attribute.ConfigurationType.ToSnapshot(), properties);
    }

    private static ConfigTypePropertyModel ToConfigTypePropertyModel(ValidSymbol<IPropertySymbol> property) => new(
        Property: property.ToSnapshot(),
        Documentation: property.XmlDocumentation.ToSnapshot(),
        IsSecret: property.HasAttribute("Secret")
    );
}