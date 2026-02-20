// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators;

using Deepstaging.EventQueue;
using Deepstaging.Projection.EventQueue;
using Deepstaging.Projection.EventQueue.Models;

/// <summary>
/// Incremental source generator for the EventQueue system.
/// Emits a <c>ChannelWorker</c> subclass with handler dispatch, effect methods
/// for <c>Enqueue</c>/<c>EnqueueWithAck</c>/<c>EnqueueAndWait</c>, and DI registration.
/// </summary>
[Generator]
public sealed class EventQueueGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var queues = context.ForAttribute<EventQueueAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryEventQueue())
            .Where(static model => model is not null)
            .Select(static (model, _) => model!);

        context.RegisterSourceOutput(queues, static (ctx, model) =>
        {
            var hint = new HintName($"{model.Namespace}.EventQueue");

            model
                .WriteEffectModule()
                .AddSourceTo(ctx, hint.Filename(model.ContainerName));
        });
    }
}
