// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Generators.Writers;
using Deepstaging.Roslyn.Generators;

namespace Deepstaging.Generators;

/// <summary>
/// Incremental source generator that produces runtime and effects code for Deepstaging-attributed types.
/// </summary>
[Generator]
public sealed class DeepstagingGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var effectsModules = context.ForAttribute<EffectsModuleAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryEffectsModules())
            .SelectMany(static (modules, _) => modules);

        context.RegisterSourceOutput(effectsModules, static (ctx, module) =>
        {
            var hint = new HintName(module.Namespace);

            module
                .WriteCapabilityInterface()
                .RegisterSourceWith(ctx, hint.Filename("Runtime", module.Capability.Interface));
            
            module
                .WriteEffectsModule()
                .RegisterSourceWith(ctx, hint.Filename("Effects", $"{module.Name}Effects"));
        });

        var runtimes = context.ForAttribute<RuntimeAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryRuntimeModel());

        context.RegisterSourceOutput(runtimes, static (ctx, module) =>
        {
            var hint = new HintName($"{module.Namespace}.Runtime");

            module
                .WriteRuntimeClass()
                .RegisterSourceWith(ctx, hint.Filename(module.RuntimeTypeName));

            module
                .WriteRuntimeBootstrapperClass()
                .RegisterSourceWith(ctx, hint.Filename($"{module.RuntimeTypeName}Bootstrapper"));
        });
    }
}