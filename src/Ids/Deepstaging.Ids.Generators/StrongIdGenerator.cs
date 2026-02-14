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
        var userTemplates = context.AdditionalTextsProvider
            .Where(static t => t.Path.EndsWith(".scriban-cs"))
            .Collect()
            .Select(static (texts, _) => UserTemplates.From(texts));

        var models = context.ForAttribute<StrongIdAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().ToStrongIdModel(ctx.SemanticModel))
            .Combine(userTemplates);

        context.RegisterSourceOutput(models, static (ctx, pair) =>
        {
            var (model, templates) = pair;
            var map = new TemplateMap<StrongIdModel>();
            model.WriteStrongId(map)
                .WithUserTemplate("Deepstaging.Ids/StrongId", model, map)
                .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName), templates);
        });
    }
}
