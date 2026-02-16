// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators;

using Config;

/// <summary>
/// Incremental source generator that generates immutable With*() methods
/// for classes marked with [Config].
/// </summary>
[Generator]
public sealed class ConfigGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all types with [Config] attribute and map to model
        var models = context.ForAttribute<ConfigRootAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryConfigModel());

        // Register source output for each model
        context.RegisterSourceOutput(models, static (ctx, model) => model
            .WriteConfigClass()
            .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName)));
    }
}
