// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators;

/// <summary>
/// Source generator that creates strongly-typed ID implementations.
/// Supports user-customizable templates via Scriban.
/// </summary>
[Generator]
public sealed class TypedIdGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<TypedIdAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().ToTypedIdModel(ctx.SemanticModel));

        context.RegisterSourceOutput(models, static (ctx, model) =>
        {
            model.WriteTypedId()
                .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName));
        });
    }
}
