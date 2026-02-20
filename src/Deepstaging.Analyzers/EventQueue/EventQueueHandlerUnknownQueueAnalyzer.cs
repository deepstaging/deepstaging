// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.EventQueue;

using Deepstaging.EventQueue;

/// <summary>
/// Reports a diagnostic when the queue name referenced by [EventQueueHandler] does not match
/// any [EventQueue] in the assembly.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "EventQueueHandler references unknown queue",
    Message = "Class '{0}' references queue '{1}' but no [EventQueue] with that name exists in the assembly",
    Description =
        "The queue name in [EventQueueHandler] must match the queue name of an [EventQueue] declared in the same assembly."
)]
public sealed class EventQueueHandlerUnknownQueueAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for unknown queue reference.
    /// </summary>
    public const string DiagnosticId = "DSEQ05";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        var handlerAttr = type.Value.GetAttributes().FirstOrDefault(a =>
            a.AttributeClass is { IsGenericType: true } cls &&
            cls.OriginalDefinition.Name == "EventQueueHandlerAttribute");

        if (handlerAttr is null) return false;

        var handlerQueueName = handlerAttr.ConstructorArguments.Length > 0
            ? handlerAttr.ConstructorArguments[0].Value as string
            : null;

        if (handlerQueueName is null) return false;

        // Search the assembly for any [EventQueue] with a matching queue name
        var compilation = type.Value.ContainingAssembly;
        return !HasMatchingQueue(compilation, handlerQueueName);
    }

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type)
    {
        var handlerAttr = type.Value.GetAttributes().First(a =>
            a.AttributeClass is { IsGenericType: true } cls &&
            cls.OriginalDefinition.Name == "EventQueueHandlerAttribute");

        var queueName = handlerAttr.ConstructorArguments[0].Value as string ?? "unknown";

        return [type.Name, queueName];
    }

    private static bool HasMatchingQueue(IAssemblySymbol assembly, string queueName)
    {
        var visitor = new QueueFinder(queueName);
        visitor.Visit(assembly.GlobalNamespace);
        return visitor.Found;
    }

    private sealed class QueueFinder(string targetQueueName) : SymbolVisitor
    {
        public bool Found { get; private set; }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            if (Found) return;

            foreach (var member in symbol.GetMembers())
                member.Accept(this);
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            if (Found) return;

            foreach (var attr in symbol.GetAttributes())
            {
                if (attr.AttributeClass?.Name != "EventQueueAttribute") continue;
                if (attr.ConstructorArguments.Length <= 0) continue;

                if (attr.ConstructorArguments[0].Value is string name && name == targetQueueName)
                {
                    Found = true;
                    return;
                }
            }
        }
    }
}
