// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Dispatch;

using Deepstaging.Dispatch;

/// <summary>
/// Reports a diagnostic when a class with [QueryHandler] is not declared as static.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "QueryHandler class must be static",
    Message = "Class '{0}' has [QueryHandler] attribute but is not declared as static",
    Description =
        "Classes decorated with [QueryHandler] must be declared as static because handler methods are static."
)]
public sealed class QueryHandlerMustBeStaticAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing static modifier on QueryHandler class.
    /// </summary>
    public const string DiagnosticId = "DSDSP04";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<QueryHandlerAttribute>() && type is { IsStatic: false };
}
