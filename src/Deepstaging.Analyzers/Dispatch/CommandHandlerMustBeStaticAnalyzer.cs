// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Dispatch;

using Deepstaging.Dispatch;

/// <summary>
/// Reports a diagnostic when a class with [CommandHandler] is not declared as static.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "CommandHandler class must be static",
    Message = "Class '{0}' has [CommandHandler] attribute but is not declared as static",
    Description =
        "Classes decorated with [CommandHandler] must be declared as static because handler methods are static."
)]
public sealed class CommandHandlerMustBeStaticAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing static modifier on CommandHandler class.
    /// </summary>
    public const string DiagnosticId = "DSDSP03";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<CommandHandlerAttribute>() && type is { IsStatic: false };
}
