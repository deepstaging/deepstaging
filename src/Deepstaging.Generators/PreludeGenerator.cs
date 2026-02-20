// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators;

/// <summary>
/// Incremental source generator that emits post-initialization output
/// such as global using directives.
/// </summary>
[Generator]
public sealed class PreludeGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            var result = GlobalUsings.Emit(
                "Deepstaging.Effects",
                LanguageExtTypes.Namespace,
                CollectionTypes.Namespace,
                TaskTypes.Namespace,
                LinqTypes.Namespace);

            if (result.IsValid(out var valid))
                ctx.AddSource("Deepstaging.GlobalUsings.g.cs", valid.Code);
        });
    }
}