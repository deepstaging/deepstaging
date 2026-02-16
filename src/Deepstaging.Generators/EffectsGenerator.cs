// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators;

using Effects;

/// <summary>
/// Incremental source generator that produces runtime and effects code for Deepstaging-attributed types.
/// </summary>
[Generator]
public sealed class EffectsGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        GenerateRuntimes(context);
        GenerateEffectModules(context);
    }

    private static void GenerateRuntimes(IncrementalGeneratorInitializationContext context)
    {
        var runtimes = context.ForAttribute<RuntimeAttribute>()
            .Map<RuntimeModel>(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryRuntimeModel());

        context.RegisterSourceOutput(runtimes, static (ctx, module) =>
        {
            var hint = new HintName($"{module.Namespace}.Runtime");

            module
                .WriteRuntimeCapabilitiesInterface()
                .AddSourceTo(ctx, hint.Filename(module.CapabilitiesInterfaceName));

            module
                .WriteRuntimeClass()
                .AddSourceTo(ctx, hint.Filename(module.RuntimeTypeName));

            module
                .WriteRuntimeBootstrapperClass()
                .AddSourceTo(ctx, hint.Filename($"{module.RuntimeTypeName}Bootstrapper"));
        });
    }

    private static void GenerateEffectModules(IncrementalGeneratorInitializationContext context)
    {
        var allEffectsModules = context.ForAttribute<EffectsModuleAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryEffectsModules())
            .SelectMany(static (modules, _) => modules);

        var dbContextModules = allEffectsModules
            .Where(static module => module.IsDbContext)
            .Collect();

        context.RegisterImplementationSourceOutput(dbContextModules, static (ctx, modules) =>
        {
            if (modules.IsDefaultOrEmpty)
                return;

            modules.WriteDbSetQueryHelper()
                .AddSourceTo(ctx, HintName.From("Deepstaging.Effects", "DbSetQuery"));
        });

        context.RegisterSourceOutput(allEffectsModules, static (ctx, effectsModule) =>
        {
            var hint = new HintName(effectsModule.Namespace);

            effectsModule
                .WriteCapabilityInterface()
                .AddSourceTo(ctx, hint.Filename("Runtime", effectsModule.Capability.Interface));

            effectsModule
                .WriteEffectsModule()
                .AddSourceTo(ctx, hint.Filename("Effects", $"{effectsModule.Name}Effects"));
        });
    }
}
