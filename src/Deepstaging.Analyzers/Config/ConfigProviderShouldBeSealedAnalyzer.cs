// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Config;

using Deepstaging.Config;

/// <summary>
/// Reports a diagnostic when a class with [ConfigProvider] is not declared as sealed.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "ConfigProvider class should be sealed",
    Message = "Class '{0}' has [ConfigProvider] attribute but is not declared as sealed",
    Description = "Classes decorated with [ConfigProvider] are typically leaf types and should be sealed to prevent inheritance.",
    Severity = DiagnosticSeverity.Warning
)]
public sealed class ConfigProviderShouldBeSealedAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing sealed modifier on ConfigProvider class.
    /// </summary>
    public const string DiagnosticId = "DSCFG02";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<ConfigProviderAttribute>() && type is { IsSealed: false, IsStatic: false };
}
