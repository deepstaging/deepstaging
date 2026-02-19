// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators;

using Effects;

/// <summary>
/// Incremental source generator for the Deepstaging effects system.
/// <para>
/// Orchestrates three independent pipelines that together produce the full effects infrastructure:
/// </para>
/// <list type="number">
///   <item>
///     <term>Runtimes</term>
///     <description>
///       Driven by <c>[Runtime]</c>. Emits the runtime class, its composite capabilities interface,
///       and the DI bootstrapper. The runtime discovers its dependencies via <c>[Uses]</c> attributes,
///       which reference container classes decorated with <c>[EffectsModule]</c> and/or <c>[Capability]</c>.
///     </description>
///   </item>
///   <item>
///     <term>Effects modules</term>
///     <description>
///       Driven by <c>[EffectsModule]</c>. Each module wraps an interface or DbContext as a LanguageExt
///       effect with OpenTelemetry instrumentation. Emits the effect class, its <c>IHas*</c> capability
///       interface, and collected XML documentation partials. DbContext modules also share a query helper.
///     </description>
///   </item>
///   <item>
///     <term>Standalone capabilities</term>
///     <description>
///       Driven by <c>[Capability]</c>. Emits <c>IHas*</c> interfaces for dependencies that don't need
///       effect wrapping (e.g., configuration providers). These interfaces are structurally identical to
///       the ones produced by effects modules, but carry no effect methods — they exist solely so the
///       runtime can express the dependency via <c>where RT : IHas*</c> constraints.
///       This pipeline must remain independent of the runtime pipeline because <c>IHas*</c> interfaces
///       must be available at compilation time regardless of whether a <c>[Runtime]</c> class exists.
///     </description>
///   </item>
/// </list>
/// </summary>
[Generator]
public sealed class EffectsGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        GenerateRuntimes(context);
        GenerateEffectModules(context);
        GenerateCapabilities(context);
    }

    /// <summary>
    /// Pipeline for <c>[Runtime]</c>-attributed classes.
    /// Emits the composite <c>ICapabilities</c> interface, the runtime partial class, and DI bootstrapper.
    /// </summary>
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

    /// <summary>
    /// Pipeline for <c>[EffectsModule]</c>-attributed classes.
    /// Each module emits its <c>IHas*</c> capability interface, the effect wrapper class,
    /// and contributes to collected XML documentation across all modules on the same container.
    /// DbContext modules additionally share a <c>DbSetQuery</c> helper.
    /// </summary>
    private static void GenerateEffectModules(IncrementalGeneratorInitializationContext context)
    {
        var allEffectsModules = context.ForAttribute<EffectsModuleAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryEffectsModules())
            .SelectMany(static (modules, _) => modules);

        var collectedEffectsModules = allEffectsModules.Collect();

        var dbContextModules = allEffectsModules
            .Where(static module => module.IsDbContext)
            .Collect();

        context.RegisterSourceOutput(dbContextModules, static (ctx, dbContexts) =>
        {
            if (dbContexts.IsDefaultOrEmpty)
                return;

            dbContexts.WriteDbSetQueryHelper()
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

        context.RegisterSourceOutput(collectedEffectsModules, static (ctx, modules) =>
        {
            if (modules.IsDefaultOrEmpty)
                return;

            foreach (var (hintName, emit) in modules.WriteEffectsDocumentation())
                emit.AddSourceTo(ctx, hintName);
        });
    }

    /// <summary>
    /// Pipeline for <c>[Capability]</c>-attributed classes.
    /// Emits <c>IHas*</c> interfaces for standalone dependencies that don't require effect wrapping.
    /// <para>
    /// This pipeline is intentionally separate from the runtime pipeline. While the runtime also
    /// discovers capabilities via <c>[Uses]</c>, the <c>IHas*</c> interfaces must exist independently
    /// — they are referenced in generic constraints (<c>where RT : IHasX</c>) that must resolve
    /// even when no <c>[Runtime]</c> class is present in the compilation.
    /// </para>
    /// </summary>
    private static void GenerateCapabilities(IncrementalGeneratorInitializationContext context)
    {
        var capabilities = context.ForAttribute<CapabilityAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryCapabilities())
            .SelectMany(static (models, _) => models);

        context.RegisterSourceOutput(capabilities, static (ctx, model) =>
        {
            var hint = new HintName(model.Namespace);

            model.Capability
                .WriteCapabilityInterface(model.Namespace)
                .AddSourceTo(ctx, hint.Filename("Runtime", model.Capability.Interface));
        });
    }
}
