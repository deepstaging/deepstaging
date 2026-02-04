// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Analyzers;

/// <summary>
/// Reports a diagnostic when a class has [Uses] attribute but not [Runtime].
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "Uses attribute requires Runtime",
    Message = "Class '{0}' has [Uses] attribute but is missing [Runtime] attribute",
    Description = "The [Uses] attribute is only meaningful on classes decorated with [Runtime]. Add [Runtime] or remove [Uses].")]
public sealed class UsesAttributeMustTargetRuntimeAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for Uses attribute without Runtime.
    /// </summary>
    public const string DiagnosticId = "DS0003";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<UsesAttribute>() && type.LacksAttribute<RuntimeAttribute>();
}
