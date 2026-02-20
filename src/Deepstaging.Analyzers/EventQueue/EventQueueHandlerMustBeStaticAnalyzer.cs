// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.EventQueue;

using Deepstaging.EventQueue;

/// <summary>
/// Reports a diagnostic when a class with [EventQueueHandler] is not declared as static.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "EventQueueHandler class must be static",
    Message = "Class '{0}' has [EventQueueHandler] attribute but is not declared as static",
    Description =
        "Classes decorated with [EventQueueHandler] must be declared as static because handler methods are static."
)]
public sealed class EventQueueHandlerMustBeStaticAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing static modifier on EventQueueHandler class.
    /// </summary>
    public const string DiagnosticId = "DSEQ03";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.Value.GetAttributes().Any(a =>
            a.AttributeClass is { IsGenericType: true } cls &&
            cls.OriginalDefinition.Name == "EventQueueHandlerAttribute") &&
        type is { IsStatic: false };
}
