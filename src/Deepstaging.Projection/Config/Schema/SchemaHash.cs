// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Config.Schema;

using System.Security.Cryptography;
using System.Text;
using Models;

/// <summary>
/// Computes a deterministic hash from a <see cref="ConfigModel"/> for staleness tracking.
/// The hash captures section name, exposed types, property names/types, and secret flags.
/// </summary>
public static class SchemaHash
{
    private const string Prefix = "deepstaging:sha256:";

    /// <summary>
    /// Computes a stable SHA-256 hash of the config model's schema-relevant data.
    /// </summary>
    public static string Compute(ConfigModel model)
    {
        var sb = new StringBuilder();
        sb.Append("section:").AppendLine(model.Section);

        foreach (var configType in model.ExposedConfigurationTypes)
        {
            sb.Append("type:").AppendLine(configType.Type.Name);

            foreach (var property in configType.Properties)
            {
                sb.Append("  prop:")
                    .Append(property.Property.Name)
                    .Append(':')
                    .Append(property.Property.Type)
                    .Append(':')
                    .Append(property.IsSecret ? "secret" : "public")
                    .Append(':')
                    .AppendLine(property.Property.IsRequired ? "required" : "optional");
            }
        }

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        return Prefix + BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Attempts to extract a hash value from a <c>$comment</c> field in schema content.
    /// Returns <c>null</c> if no valid hash is found.
    /// </summary>
    public static string? Extract(string schemaContent)
    {
        // Look for: "$comment": "deepstaging:sha256:..."
        const string marker = "\"$comment\"";
        var idx = schemaContent.IndexOf(marker, StringComparison.Ordinal);

        if (idx < 0)
            return null;

        var colonIdx = schemaContent.IndexOf(':', idx + marker.Length);

        if (colonIdx < 0)
            return null;

        // Find the opening quote of the value
        var openQuote = schemaContent.IndexOf('"', colonIdx + 1);

        if (openQuote < 0)
            return null;

        var closeQuote = schemaContent.IndexOf('"', openQuote + 1);

        if (closeQuote < 0)
            return null;

        var value = schemaContent.Substring(openQuote + 1, closeQuote - openQuote - 1);

        return value.StartsWith(Prefix, StringComparison.Ordinal) ? value : null;
    }
}
