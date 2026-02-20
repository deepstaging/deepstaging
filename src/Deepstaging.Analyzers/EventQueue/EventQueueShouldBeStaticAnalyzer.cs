// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.EventQueue;

using Deepstaging.EventQueue;

/// <summary>
/// Reports a diagnostic when a class with [EventQueue] is not declared as static.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "EventQueue class must be static",
    Message = "Class '{0}' has [EventQueue] attribute but is not declared as static",
    Description =
        "Classes decorated with [EventQueue] must be declared as static because the generated effect methods are static members.",
    Severity = DiagnosticSeverity.Warning
)]
public sealed class EventQueueShouldBeStaticAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing static modifier on EventQueue class.
    /// </summary>
    public const string DiagnosticId = "DSEQ02";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<EventQueueAttribute>() && type is { IsStatic: false };
}
