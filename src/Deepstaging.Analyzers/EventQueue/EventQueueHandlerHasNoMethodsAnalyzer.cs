// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.EventQueue;

using Deepstaging.EventQueue;

/// <summary>
/// Reports a diagnostic when an [EventQueueHandler] has no valid handler methods.
/// Valid handler methods must be static and accept exactly one parameter (the event type).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "EventQueueHandler has no handler methods",
    Message = "Class '{0}' has [EventQueueHandler] attribute but contains no valid handler methods",
    Description =
        "Classes decorated with [EventQueueHandler] must contain at least one static method that accepts a single event parameter.",
    Severity = DiagnosticSeverity.Warning
)]
public sealed class EventQueueHandlerHasNoMethodsAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for EventQueueHandler with no valid handler methods.
    /// </summary>
    public const string DiagnosticId = "DSEQ04";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        var hasAttribute = type.Value.GetAttributes().Any(a =>
            a.AttributeClass is { IsGenericType: true } cls &&
            cls.OriginalDefinition.Name == "EventQueueHandlerAttribute");

        if (!hasAttribute) return false;

        var hasHandlerMethods = type.QueryMethods()
            .Where(m => m.IsStatic && m.Parameters.Length == 1)
            .GetAll()
            .Length > 0;

        return !hasHandlerMethods;
    }
}
