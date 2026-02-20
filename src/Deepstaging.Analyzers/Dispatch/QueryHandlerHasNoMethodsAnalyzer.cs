// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Dispatch;

using Deepstaging.Dispatch;

/// <summary>
/// Reports a diagnostic when a [QueryHandler] class has no valid handler methods.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "QueryHandler has no handler methods",
    Message = "Class '{0}' has [QueryHandler] attribute but contains no valid handler methods",
    Description =
        "Classes decorated with [QueryHandler] must contain at least one static method with a query parameter.",
    Severity = DiagnosticSeverity.Warning
)]
public sealed class QueryHandlerHasNoMethodsAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for QueryHandler with no handler methods.
    /// </summary>
    public const string DiagnosticId = "DSDSP06";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        if (!type.HasAttribute<QueryHandlerAttribute>()) return false;

        var hasHandlerMethods = type.QueryMethods()
            .Where(m => m.IsStatic && m.Parameters.Length >= 1)
            .GetAll()
            .Length > 0;

        return !hasHandlerMethods;
    }
}
