// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Ids;

/// <summary>
/// Reports a diagnostic when a struct with [TypedId] is not declared as readonly.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "TypedId struct should be readonly",
    Message = "Struct '{0}' has [TypedId] attribute but is not declared as readonly",
    Description =
        "Structs decorated with [TypedId] should be declared as readonly for better performance and to prevent accidental mutation.",
    Severity = DiagnosticSeverity.Warning
)]
public sealed class TypedIdShouldBeReadonlyAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing readonly modifier on TypedId struct.
    /// </summary>
    public const string DiagnosticId = "DSID02";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<TypedIdAttribute>() && type is { IsReadOnly: false };
}