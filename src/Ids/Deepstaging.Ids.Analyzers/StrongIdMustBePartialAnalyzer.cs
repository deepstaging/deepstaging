// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Ids.Analyzers;

/// <summary>
/// Reports a diagnostic when a struct with [StrongId] is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "StrongId struct must be partial",
    Message = "Struct '{0}' has [StrongId] attribute but is not declared as partial",
    Description =
        "Structs decorated with [StrongId] must be declared as partial because the source generator emits additional partial struct members.")]
public sealed class StrongIdMustBePartialAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing partial modifier on StrongId struct.
    /// </summary>
    public const string DiagnosticId = "ID0001";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<StrongIdAttribute>() && type is { IsPartial: false };
}
