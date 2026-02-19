// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Config;

using Deepstaging.Config;
using Deepstaging.Projection.Config.Schema;
using System.Collections.Generic;

/// <summary>
/// Reports a diagnostic when a class with [ConfigProvider] is missing configuration files
/// or when existing files are out of date with the current model.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[TracksFiles(DiagnosticId,
    MissingTitle = "Configuration files should be generated",
    MissingMessage = "Class '{0}' is missing configuration files: {1}",
    MissingDescription = "Classes decorated with [ConfigProvider] should have corresponding JSON schema files " +
        "(deepstaging.schema.json and user-secrets.schema.json) to enable configuration validation.",
    StaleTitle = "Configuration files are out of date",
    StaleMessage = "Configuration files for '{0}' are out of date ({1}) — regenerate to match current model",
    StaleDescription = "The JSON schema files for this [ConfigProvider] no longer match the current configuration model. " +
        "Regenerate them to keep validation accurate.")]
public sealed class ConfigProviderSchemaAnalyzer : TrackedFileTypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing or stale configuration files on ConfigProvider class.
    /// </summary>
    public const string DiagnosticId = "DSCFG06";

    /// <inheritdoc />
    protected override bool IsTrackedFile(string filePath) =>
        filePath.EndsWith(".schema.json", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    protected override bool IsRelevant(ValidSymbol<INamedTypeSymbol> symbol) =>
        symbol.HasAttribute<ConfigProviderAttribute>();

    /// <inheritdoc />
    protected override string ComputeHash(ValidSymbol<INamedTypeSymbol> symbol) =>
        SchemaHash.Compute(symbol.QueryConfigModel());

    /// <inheritdoc />
    protected override string? ExtractHash(string fileContent) =>
        SchemaHash.Extract(fileContent);

    /// <inheritdoc />
    protected override IEnumerable<string> GetExpectedFileNames(ValidSymbol<INamedTypeSymbol> symbol)
    {
        yield return "deepstaging.schema.json";

        if (symbol.QueryConfigModel().HasSecrets)
            yield return "user-secrets.schema.json";
    }

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> symbol) =>
        [symbol.Name, string.Join(", ", GetExpectedFileNames(symbol))];
}
