// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators;

using Config;

/// <summary>
/// Incremental source generator that generates strongly-typed configuration providers
/// for classes marked with <see cref="ConfigProviderAttribute"/>.
/// </summary>
[Generator]
public sealed class ConfigGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<ConfigProviderAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryConfigModel());

        context.RegisterSourceOutput(models, static (ctx, model) => model
            .WriteConfigClass()
            .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName)));
    }
}
