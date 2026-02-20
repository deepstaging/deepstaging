// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators;

using Deepstaging.Dispatch;
using Deepstaging.Projection.Dispatch;
using Deepstaging.Projection.Dispatch.Models;

/// <summary>
/// Incremental source generator for the Dispatch system.
/// Emits strongly-typed dispatch method overloads as direct <c>Eff</c> compositions —
/// one per command/query handler. No runtime routing or lookup occurs.
/// </summary>
[Generator]
public sealed class DispatchGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var modules = context.ForAttribute<DispatchModuleAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryDispatchModule())
            .Where(static model => model is not null)
            .Select(static (model, _) => model!);

        // Discover command handlers assembly-wide
        var commandHandlers = context.ForAttribute<CommandHandlerAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryCommandHandlerGroup())
            .Where(static group => group is not null)
            .Select(static (group, _) => group!)
            .Collect();

        // Discover query handlers assembly-wide
        var queryHandlers = context.ForAttribute<QueryHandlerAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryQueryHandlerGroup())
            .Where(static group => group is not null)
            .Select(static (group, _) => group!)
            .Collect();

        // Combine module with discovered handlers
        var combined = modules
            .Combine(commandHandlers)
            .Combine(queryHandlers)
            .Select(static (tuple, _) =>
            {
                var ((module, commands), queries) = tuple;
                return module with
                {
                    CommandHandlers = commands,
                    QueryHandlers = queries
                };
            });

        context.RegisterSourceOutput(combined, static (ctx, model) =>
        {
            var hint = new HintName($"{model.Namespace}.Dispatch");

            model
                .WriteDispatchModule()
                .AddSourceTo(ctx, hint.Filename(model.ContainerName));
        });
    }
}
