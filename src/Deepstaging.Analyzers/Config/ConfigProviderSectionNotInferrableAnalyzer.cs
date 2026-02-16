// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Config;

using Deepstaging.Config;
using Deepstaging.Projection.Config;

/// <summary>
/// Reports a diagnostic when the configuration section name cannot be determined —
/// the class name does not end with <c>ConfigProvider</c> and no explicit <c>Section</c> is provided.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "ConfigProvider section could not be inferred",
    Message = "Class '{0}' has [ConfigProvider] but section name could not be inferred. Provide an explicit Section or use a 'ConfigProvider' suffix.",
    Description =
        "The configuration section name is inferred by stripping the 'ConfigProvider' suffix from the class name. " +
        "If the class name does not end with 'ConfigProvider', an explicit Section must be provided."
)]
public sealed class ConfigProviderSectionNotInferrableAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for non-inferrable section name on ConfigProvider class.
    /// </summary>
    public const string DiagnosticId = "CFG003";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<ConfigProviderAttribute>() &&
        string.IsNullOrEmpty(type.ConfigProviderAttribute().GetSectionName(type));
}
