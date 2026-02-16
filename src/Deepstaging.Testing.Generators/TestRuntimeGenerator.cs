// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Testing.Generators.Writers;
using Deepstaging.Roslyn.Generators;

namespace Deepstaging.Testing.Generators;

/// <summary>
/// Incremental source generator that produces test runtime implementations
/// for types decorated with <see cref="TestRuntimeAttribute{TRuntime}"/>.
/// </summary>
[Generator]
public sealed class TestRuntimeGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var runtimes = context
            .ForAttribute(typeof(TestRuntimeAttribute<>))
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryTestRuntimeModel());

        context.RegisterSourceOutput(runtimes, static (ctx, model) => model
            .WriteTestRuntimeClass()
            .AddSourceTo(ctx, HintName.From(model.Namespace, model.TestRuntimeType.Name)));
    }
}
