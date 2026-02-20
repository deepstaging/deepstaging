// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Dispatch;

using Deepstaging.Dispatch;

/// <summary>
/// Reports a diagnostic when a [CommandHandler] class has no valid handler methods.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "CommandHandler has no handler methods",
    Message = "Class '{0}' has [CommandHandler] attribute but contains no valid handler methods",
    Description =
        "Classes decorated with [CommandHandler] must contain at least one static method with a command parameter.",
    Severity = DiagnosticSeverity.Warning
)]
public sealed class CommandHandlerHasNoMethodsAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for CommandHandler with no handler methods.
    /// </summary>
    public const string DiagnosticId = "DSDSP05";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        if (!type.HasAttribute<CommandHandlerAttribute>()) return false;

        var hasHandlerMethods = type.QueryMethods()
            .Where(m => m.IsStatic && m.Parameters.Length >= 1)
            .GetAll()
            .Length > 0;

        return !hasHandlerMethods;
    }
}
