// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Config.Schema;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

/// <summary>
/// Builds JSON Schema Draft-7 documents from <see cref="ConfigModel"/> data.
/// </summary>
public static class JsonSchemaBuilder
{
    /// <summary>
    /// Builds an appsettings JSON Schema containing only non-secret properties.
    /// </summary>
    public static string BuildAppSettingsSchema(ConfigModel model) =>
        BuildSchema(model, secret: false);

    /// <summary>
    /// Builds a secrets JSON Schema containing only secret-marked properties.
    /// </summary>
    public static string BuildSecretsSchema(ConfigModel model) =>
        BuildSchema(model, secret: true);

    private static string BuildSchema(ConfigModel model, bool secret)
    {
        var hash = SchemaHash.Compute(model);
        var id = secret ? "user-secrets" : "deepstaging";
        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine("  \"$schema\": \"https://json-schema.org/draft-07/schema#\",");
        sb.Append("  \"$id\": \"").Append(id).AppendLine(".schema.json\",");
        sb.Append("  \"$comment\": \"").Append(hash).AppendLine("\",");
        sb.AppendLine("  \"type\": \"object\",");
        sb.AppendLine("  \"properties\": {");
        sb.Append("    \"").Append(model.Section).AppendLine("\": {");
        sb.AppendLine("      \"type\": \"object\",");
        sb.AppendLine("      \"properties\": {");

        var sectionProperties = new List<string>();

        foreach (var configType in model.ExposedConfigurationTypes)
        {
            var typeProperties = GetProperties(configType, secret);
            if (typeProperties.Count == 0)
                continue;

            sectionProperties.Add(BuildConfigTypeSchema(configType, typeProperties));
        }

        sb.AppendLine(string.Join(",\n", sectionProperties));
        sb.AppendLine("      }");
        sb.AppendLine("    }");
        sb.AppendLine("  }");
        sb.Append("}");
        return sb.ToString();
    }

    private static string BuildConfigTypeSchema(ConfigTypeModel configType, List<ConfigTypePropertyModel> properties)
    {
        var sb = new StringBuilder();
        sb.Append("        \"").Append(configType.Type.Name).AppendLine("\": {");
        sb.AppendLine("          \"type\": \"object\",");
        sb.AppendLine("          \"properties\": {");

        var propertySchemas = properties
            .Select(BuildPropertySchema)
            .ToList();

        sb.AppendLine(string.Join(",\n", propertySchemas));
        sb.AppendLine("          },");

        var required = properties
            .Where(p => p.Property.IsRequired)
            .Select(p => "\"" + p.Property.Name + "\"");

        sb.Append("          \"required\": [").Append(string.Join(", ", required)).AppendLine("],");
        sb.AppendLine("          \"additionalProperties\": false");
        sb.Append("        }");
        return sb.ToString();
    }

    private static string BuildPropertySchema(ConfigTypePropertyModel property)
    {
        var jsonType = MapToJsonSchemaType(property.Property.Type);
        var sb = new StringBuilder();

        sb.Append("            \"").Append(property.Property.Name).Append("\": { \"type\": \"").Append(jsonType).Append("\"");

        var summary = property.Documentation.Summary;
        if (!string.IsNullOrEmpty(summary))
        {
            var escaped = summary!
                .Replace("\"", "\\\"")
                .Replace("\n", " ")
                .Trim();
            sb.Append(", \"description\": \"").Append(escaped).Append("\"");
        }

        sb.Append(" }");
        return sb.ToString();
    }

    /// <summary>
    /// Maps a C# type name to a JSON Schema type string.
    /// </summary>
    public static string MapToJsonSchemaType(string csharpType)
    {
        var type = csharpType
            .Replace("global::", "")
            .TrimEnd('?');

        return type switch
        {
            "System.String" or "string" => "string",
            "System.Int32" or "int" => "integer",
            "System.Int64" or "long" => "integer",
            "System.Double" or "double" => "number",
            "System.Single" or "float" => "number",
            "System.Decimal" or "decimal" => "number",
            "System.Boolean" or "bool" => "boolean",
            "System.DateTime" or "System.DateTimeOffset" => "string",
            "System.Guid" => "string",
            "System.Uri" => "string",
            "System.Byte[]" => "string",
            _ => "object"
        };
    }

    private static List<ConfigTypePropertyModel> GetProperties(ConfigTypeModel configType, bool secret) =>
        configType.Properties.Where(p => p.IsSecret == secret).ToList();
}
