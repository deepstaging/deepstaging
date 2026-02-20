// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Dispatch;

using Deepstaging.Dispatch;

/// <summary>
/// Reports a diagnostic when a class with [DispatchModule] is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "DispatchModule class must be partial",
    Message = "Class '{0}' has [DispatchModule] attribute but is not declared as partial",
    Description =
        "Classes decorated with [DispatchModule] must be declared as partial because the source generator emits additional partial class members."
)]
public sealed class DispatchModuleMustBePartialAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing partial modifier on DispatchModule class.
    /// </summary>
    public const string DiagnosticId = "DSDSP01";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<DispatchModuleAttribute>() && type is { IsPartial: false };
}
