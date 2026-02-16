// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Config;

using Deepstaging.Config;

/// <summary>
/// Reports a diagnostic when a class with [ConfigProvider] does not have corresponding
/// JSON schema files, prompting the user to generate them via a code fix.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "Configuration schema files should be generated",
    Message = "Class '{0}' has [ConfigProvider] — generate appsettings and secrets schema files",
    Description =
        "Classes decorated with [ConfigProvider] should have corresponding JSON schema files " +
        "(appsettings.schema.json and secrets.schema.json) to enable configuration validation.",
    Severity = DiagnosticSeverity.Info
)]
public sealed class ConfigProviderSchemaAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing schema files on ConfigProvider class.
    /// </summary>
    public const string DiagnosticId = "CFG006";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<ConfigProviderAttribute>();
}
