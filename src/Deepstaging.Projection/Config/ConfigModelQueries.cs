// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Config;

using Attributes;
using Models;

/// <summary>
/// Provides extension methods for building configuration models from type symbols.
/// </summary>
public static class ConfigModelQueries
{
    /// <summary>
    /// The default data directory for configuration files.
    /// </summary>
    public const string DefaultDataDirectory = ".config";

    /// <summary>
    /// The MSBuild property name for the data directory.
    /// </summary>
    public const string DataDirectoryPropertyName = "DeepstagingDataDirectory";

    /// <summary>
    /// </summary>
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Builds a <see cref="Models.ConfigModel"/> from the current symbol, extracting relevant information
        /// such as namespace, type name, accessibility, section, and exposed configuration types.
        /// </summary>
        /// <param name="dataDirectory">
        /// The data directory for configuration files, typically from the
        /// <c>DeepstagingDataDirectory</c> MSBuild property. Defaults to <c>.config</c>.
        /// </param>
        public ConfigModel QueryConfigModel(string? dataDirectory = null)
        {
            var attribute = symbol.ConfigProviderAttribute();

            return new ConfigModel
            {
                Namespace = symbol.Namespace ?? "",
                TypeName = symbol.Name,
                Accessibility = symbol.AccessibilityString,
                Section = attribute.GetSectionName(symbol),
                DataDirectory = dataDirectory ?? DefaultDataDirectory,
                ExposedConfigurationTypes =
                [
                    ..symbol.ExposesAttributes()
                        .Select(ToConfigTypeModel)
                ]
            };
        }
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
        IsSecret: property.HasAttribute<SecretAttribute>()
    );
}