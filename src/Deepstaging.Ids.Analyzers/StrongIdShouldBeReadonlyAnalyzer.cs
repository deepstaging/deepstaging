// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Ids.Analyzers;

/// <summary>
/// Reports a diagnostic when a struct with [StrongId] is not declared as readonly.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "StrongId struct should be readonly",
    Message = "Struct '{0}' has [StrongId] attribute but is not declared as readonly",
    Description =
        "Structs decorated with [StrongId] should be declared as readonly for better performance and to prevent accidental mutation.",
    Severity = DiagnosticSeverity.Warning)]
public sealed class StrongIdShouldBeReadonlyAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing readonly modifier on StrongId struct.
    /// </summary>
    public const string DiagnosticId = "ID0002";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<StrongIdAttribute>() && type is { IsReadOnly: false };
}
