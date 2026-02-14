// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Generators;
using Deepstaging.Roslyn.Scriban;
using Microsoft.CodeAnalysis;

namespace Deepstaging.Ids.Generators;

/// <summary>
/// Source generator that creates strongly-typed ID implementations.
/// Supports user-customizable templates via Scriban.
/// </summary>
[Generator]
public sealed class StrongIdGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var userTemplates = context.UserTemplatesProvider;

        var models = context.ForAttribute<StrongIdAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().ToStrongIdModel(ctx.SemanticModel));

        // Per-model: generate source (uses user template if available)
        context.RegisterSourceOutput(models.Combine(userTemplates), static (ctx, pair) =>
        {
            var (model, templates) = pair;
            var map = new TemplateMap<StrongIdModel>();

            model.WriteStrongId(map)
                .WithUserTemplate("Deepstaging.Ids/StrongId", model, map)
                .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName), templates);
        });

        // Once: emit scaffold metadata from first model
        context.RegisterSourceOutput(
            models.Collect().Select(static (m, _) => m.Length > 0 ? m[0] : default),
            static (ctx, model) =>
            {
                if (model is null) return;

                var map = new TemplateMap<StrongIdModel>();
                var customizable = model.WriteStrongId(map)
                    .WithUserTemplate("Deepstaging.Ids/StrongId", model, map);

                ScaffoldEmitter.EmitScaffold(ctx, customizable,
                    "Deepstaging.Ids.StrongIdAttribute");
            });
    }
}
