using Deepstaging.Data;
using Deepstaging.Generators.Emitters;

namespace Deepstaging.Generators;

/// <summary>
/// Single incremental generator for all Deepstaging.Effects features.
/// Uses standalone modules with [Uses] for composition.
/// </summary>
[Generator]
public sealed class DeepstagingGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var effectsModules = context.ForAttribute<EffectsModuleAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().ReadEffectsModules())
            .SelectMany(static (modules, _) => modules);

        context.RegisterSourceOutput(effectsModules, static (ctx, module) =>
        {
            ctx.EmitCapabilityInterface(module);
            ctx.EmitEffectsModule(module);
        });

        // Pipeline 4: Discover standalone [EventQueueModule] attributes
        // var standaloneEventQueues = context.ForAttribute<EventQueueModuleAttribute>()
        //     .Map(static (ctx, _) => ctx.TargetSymbol.QueryEventQueueModule());
        //
        // // Pipeline 5: Discover standalone [DispatcherModule] attributes
        // var standaloneDispatchers = context.SyntaxProvider
        //     .ForAttributeWithMetadataName(
        //         "Deepstaging.Effects.DispatcherModuleAttribute",
        //         predicate: static (node, _) => node is ClassDeclarationSyntax,
        //         transform: static (ctx, _) => DispatcherModuleDiscovery.Build(ctx))
        //     .Where(static m => m is not null)
        //     .Select(static (m, _) => m!);
        //
        // // Emit standalone dispatcher modules
        // context.RegisterSourceOutput(standaloneDispatchers, static (ctx, module) =>
        // {
        //     DispatcherModuleEmitter.EmitCapabilityInterface(ctx, module);
        //     DispatcherModuleEmitter.EmitDispatcherClass(ctx, module);
        //
        //     // Report duplicate handler warnings
        //     foreach (var diag in module.Diagnostics)
        //     {
        //         ctx.ReportDiagnostic(Diagnostic.Create(
        //             new DiagnosticDescriptor(
        //                 "DEEPEFF003",
        //                 "Duplicate command handlers",
        //                 "Multiple handlers found for {0}. Using {1}, ignoring: {2}",
        //                 "Deepstaging.Effects",
        //                 DiagnosticSeverity.Warning,
        //                 isEnabledByDefault: true),
        //             Location.None,
        //             diag.CommandType,
        //             diag.UsedHandler,
        //             string.Join(", ", diag.IgnoredHandlers)));
        //     }
        // });
        //
        // var eventHandlers = context.ForAttribute<EventQueueHandlerAttribute>()
        //     .Map(static (ctx, _) => ctx.TargetSymbol.QueryEventQueueHandlers())
        //     .SelectMany(static (handlers, _) => handlers);
        //
        // // Collect all event handlers
        // var allEventHandlers = eventHandlers.Collect();
        //
        // // Emit standalone event queue modules with handlers
        // var standaloneEventQueuesWithHandlers = standaloneEventQueues
        //     .Combine(allEventHandlers);
        //
        // context.RegisterSourceOutput(standaloneEventQueuesWithHandlers, static (ctx, tuple) =>
        // {
        //     var (module, handlers) = tuple;
        //
        //     // Find handlers for this queue
        //     var queueHandlers = handlers
        //         .Where(h => h.QueueName == module.Name)
        //         .ToImmutableArray();
        //
        //     EventQueueModuleEmitter.EmitCapabilityInterface(ctx, module);
        //     EventQueueModuleEmitter.EmitEffectsModule(ctx, module);
        //     EventQueueModuleEmitter.EmitQueueService(ctx, module, queueHandlers);
        // });

        // var runtimes = context.ForAttribute<RuntimeAttribute>()
        //     .Map(static (ctx, _) => ctx.TargetSymbol.QueryRuntimeModel());

        // // Emit runtime code with dependencies from [Uses] attributes
        // context.RegisterSourceOutput(runtimes, static (ctx, runtime) =>
        // {
        //     // Build dependencies from [Uses] attributes (standalone modules)
        //     // Note: Dispatcher modules don't add dependencies (they use generic constraints)
        //     var dependencies = runtime.UsedModuleReferences
        //         .Where(mod => mod.ModuleKind != ModuleKind.Dispatcher)
        //         .Select(mod => new DependencyModel
        //         {
        //             FullyQualifiedName = mod.TargetType,
        //             TypeName = mod.TargetTypeName,
        //             PropertyName = mod.PropertyName,
        //             ParameterName = mod.ParameterName,
        //             SourceFeature = $"Uses.{mod.ModuleName}"
        //         })
        //         .ToImmutableArray();
        //
        //     // Build capability interface names from [Uses] modules
        //     var capabilityInterfaces = runtime.UsedModuleReferences
        //         .Select(m => m.CapabilityInterface)
        //         .ToImmutableArray();
        //
        //     // Build final model with aggregated data
        //     var finalModel = runtime with
        //     {
        //         Dependencies = dependencies,
        //         CapabilityInterfaces = capabilityInterfaces
        //     };
        //
        //     // Emit runtime code
        //     RuntimeEmitter.EmitCore(ctx, finalModel);
        //     RuntimeEmitter.EmitDI(ctx, finalModel);
        // });
    }
}