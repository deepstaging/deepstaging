// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Generators.Writers;
using Deepstaging.Roslyn.Generators;

namespace Deepstaging.Generators;

/// <summary>
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

            ctx.AddFromEmit(hint.Filename("Runtime", module.Capability.Interface), module.WriteCapabilityInterface());
            ctx.AddFromEmit(hint.Filename("Effects", $"{module.Name}Effects"), module.WriteEffectsModule());
        });

        var runtimes = context.ForAttribute<RuntimeAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryRuntimeModel());

        context.RegisterSourceOutput(runtimes, static (ctx, module) =>
        {
            var hint = new HintName($"{module.Namespace}.Runtime");
            ctx.AddFromEmit(hint.Filename(module.RuntimeTypeName), module.WriteRuntimeClass());
            ctx.AddFromEmit(hint.Filename($"{module.RuntimeTypeName}Bootstrapper"),
                module.WriteRuntimeBootstrapperClass());
        });
    }
}