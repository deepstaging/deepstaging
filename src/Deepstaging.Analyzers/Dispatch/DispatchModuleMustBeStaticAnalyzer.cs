// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Dispatch;

using Deepstaging.Dispatch;

/// <summary>
/// Reports a diagnostic when a class with [DispatchModule] is not declared as static.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "DispatchModule class must be static",
    Message = "Class '{0}' has [DispatchModule] attribute but is not declared as static",
    Description =
        "Classes decorated with [DispatchModule] must be declared as static because all dispatch methods are static."
)]
public sealed class DispatchModuleMustBeStaticAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing static modifier on DispatchModule class.
    /// </summary>
    public const string DiagnosticId = "DSDSP02";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<DispatchModuleAttribute>() && type is { IsStatic: false };
}
