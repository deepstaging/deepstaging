// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Effects;

using Deepstaging.Effects;

/// <summary>
/// Reports a diagnostic when a class with [EffectsModule] is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "EffectsModule class must be partial",
    Message = "Class '{0}' has [EffectsModule] attribute but is not declared as partial",
    Description =
        "Classes decorated with [EffectsModule] must be declared as partial because the source generator emits additional partial class members."
)]
public sealed class EffectsModuleMustBePartialAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing partial modifier on EffectsModule class.
    /// </summary>
    public const string DiagnosticId = "DSEFX01";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<EffectsModuleAttribute>() && type is { IsPartial: false };
}