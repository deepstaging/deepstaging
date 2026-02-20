// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.EventQueue;

using Deepstaging.EventQueue;

/// <summary>
/// Reports a diagnostic when a class with [EventQueue] is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "EventQueue class must be partial",
    Message = "Class '{0}' has [EventQueue] attribute but is not declared as partial",
    Description =
        "Classes decorated with [EventQueue] must be declared as partial because the source generator emits additional partial class members."
)]
public sealed class EventQueueMustBePartialAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing partial modifier on EventQueue class.
    /// </summary>
    public const string DiagnosticId = "DSEQ01";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<EventQueueAttribute>() && type is { IsPartial: false };
}
