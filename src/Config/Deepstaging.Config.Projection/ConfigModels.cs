// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Config.Projection.Models;

namespace Deepstaging.Config.Projection;

/// <summary>
/// Provides extension methods for querying and building "With" method models from type symbols.
/// </summary>
public static class ConfigModels
{
    /// <summary>
    /// </summary>
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConfigModel QueryConfigModel()
        {
            return new ConfigModel
            {
                Namespace = symbol.Namespace ?? "",
                TypeName = symbol.Name,
                Accessibility = symbol.Accessibility,
                ExposedConfigurationTypes = symbol.ExposesAttributes()
                    .Select(x => x.ConfigurationType.ToSnapshot())
                    .ToEquatableArray()
            };
        }
    }
}
