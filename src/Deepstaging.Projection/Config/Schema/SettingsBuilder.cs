// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Config.Schema;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

/// <summary>
/// Builds template JSON settings files from <see cref="ConfigModel"/> data.
/// Used by the code fix to scaffold and merge settings files with the correct structure.
/// </summary>
public static class SettingsBuilder
{
    /// <summary>
    /// Builds a settings JSON template containing only non-secret properties.
    /// Includes a <c>$schema</c> reference to <paramref name="schemaRef"/>.
    /// </summary>
    /// <param name="model">The configuration model to build settings from.</param>
    /// <param name="schemaRef">The <c>$schema</c> URI to embed in the JSON output.</param>
    public static string BuildSettings(ConfigModel model, string schemaRef) =>
        Build(model, secret: false, schemaRef);

    /// <summary>
    /// Builds a secrets JSON template containing only secret-marked properties.
    /// Includes a <c>$schema</c> reference to <paramref name="schemaRef"/>.
    /// </summary>
    /// <param name="model">The configuration model to build secrets from.</param>
    /// <param name="schemaRef">The <c>$schema</c> URI to embed in the JSON output.</param>
    public static string BuildSecrets(ConfigModel model, string schemaRef) =>
        Build(model, secret: true, schemaRef);

    private static string Build(ConfigModel model, bool secret, string schemaRef)
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.Append("  \"$schema\": \"").Append(schemaRef).AppendLine("\",");
        sb.Append("  \"").Append(model.Section).AppendLine("\": {");

        var typeEntries = new List<string>();

        foreach (var configType in model.ExposedConfigurationTypes)
        {
            var properties = configType.Properties.Where(p => p.IsSecret == secret).ToList();

            if (properties.Count == 0)
                continue;

            typeEntries.Add(BuildConfigType(configType, properties));
        }

        sb.AppendLine(string.Join(",\n", typeEntries));
        sb.AppendLine("  }");
        sb.Append("}");
        return sb.ToString();
    }

    private static string BuildConfigType(ConfigTypeModel configType, List<ConfigTypePropertyModel> properties)
    {
        var sb = new StringBuilder();
        sb.Append("    \"").Append(configType.Type.Name).AppendLine("\": {");

        var propertyEntries = properties
            .Select(BuildProperty)
            .ToList();

        sb.Append(string.Join(",\n", propertyEntries)).AppendLine();
        sb.Append("    }");
        return sb.ToString();
    }

    private static string BuildProperty(ConfigTypePropertyModel property)
    {
        var defaultValue = GetDefaultValue(property.Property.Type);
        return $"      \"{property.Property.Name}\": {defaultValue}";
    }

    private static string GetDefaultValue(string csharpType)
    {
        var type = csharpType
            .Replace("global::", "")
            .TrimEnd('?');

        return type switch
        {
            "System.String" or "string" => "\"\"",
            "System.Int32" or "int" => "0",
            "System.Int64" or "long" => "0",
            "System.Double" or "double" => "0.0",
            "System.Single" or "float" => "0.0",
            "System.Decimal" or "decimal" => "0.0",
            "System.Boolean" or "bool" => "false",
            "System.DateTime" or "System.DateTimeOffset" => "\"\"",
            "System.Guid" => "\"00000000-0000-0000-0000-000000000000\"",
            "System.Uri" => "\"\"",
            _ => "{}"
        };
    }
}