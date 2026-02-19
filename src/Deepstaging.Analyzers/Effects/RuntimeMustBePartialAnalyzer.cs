// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Effects;

using Deepstaging.Effects;

/// <summary>
/// Reports a diagnostic when a class with [Runtime] is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "Runtime class must be partial",
    Message = "Class '{0}' has [Runtime] attribute but is not declared as partial",
    Description =
        "Classes decorated with [Runtime] must be declared as partial because the source generator emits additional partial class members."
)]
public sealed class RuntimeMustBePartialAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing partial modifier on Runtime class.
    /// </summary>
    public const string DiagnosticId = "DSRT01";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<RuntimeAttribute>() && type is { IsPartial: false };
}