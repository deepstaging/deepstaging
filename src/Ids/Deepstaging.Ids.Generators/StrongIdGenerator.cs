// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Generators;
using Microsoft.CodeAnalysis;

namespace Deepstaging.Ids.Generators;

/// <summary>
/// Source generator that creates strongly-typed ID implementations.
/// </summary>
[Generator]
public sealed class StrongIdGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<StrongIdAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().ToStrongIdModel(ctx.SemanticModel));

        context.RegisterSourceOutput(models, static (ctx, model) =>
            model.WriteStrongId()
                .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName)));
    }
}
