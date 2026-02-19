// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Config;

using Deepstaging.Config;

/// <summary>
/// Reports a diagnostic when a class with [ConfigProvider] is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "ConfigProvider class must be partial",
    Message = "Class '{0}' has [ConfigProvider] attribute but is not declared as partial",
    Description =
        "Classes decorated with [ConfigProvider] must be declared as partial because the source generator emits additional partial class members."
)]
public sealed class ConfigProviderMustBePartialAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing partial modifier on ConfigProvider class.
    /// </summary>
    public const string DiagnosticId = "DSCFG01";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<ConfigProviderAttribute>() && type is { IsPartial: false };
}
