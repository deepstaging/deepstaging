using Deepstaging.Data.Models;
using Deepstaging.Roslyn.Emit;

namespace Deepstaging.Generators.Emitters;

/// <summary>
/// Emits code for standalone [EffectsModule] decorated classes.
/// </summary>
public static partial class EffectsModuleEmitter
{
    extension(EffectsModuleModel model)
    {
        /// <summary>
        /// Emits the capability interface (e.g., IHasAppDbContext).
        /// </summary>
        public OptionalEmit EmitCapabilityInterface() => TypeBuilder
            .Interface(model.CapabilityInterface)
            .InNamespace(model.Namespace)
            .AddProperty(model.PropertyName, model.TargetType, builder => builder.AsReadOnly())
            .Emit();

        /// <summary>
        /// Emits the effects module with all effect methods.
        /// </summary>
        public OptionalEmit EmitEffectsModule()
        {
            var module = TypeBuilder.Parse($"public static partial class {model.Name}")
                .AddInstrumentationActivitySource(model)
                .AddEffectMethods(model)
                .AddDbContextEffects(model);

            return TypeBuilder.Parse($"public static partial class {model.ClassName}")
                .AddUsing("Deepstaging")
                .InNamespace(model.Namespace)
                .AddNestedType(module)
                .Emit();
        }
    }

    private static readonly Func<string, TemplateName> Named =
        TemplateName.ForGenerator<DeepstagingGenerator>();

    /// <summary>
    /// Emits the capability interface for a standalone effects module.
    /// </summary>
    public static void EmitCapabilityInterface(this SourceProductionContext context, EffectsModuleModel module)
    {
        context.AddFromEmit(
            hintName: new HintNameProvider(module.Namespace).Filename(module.CapabilityInterface),
            emit: module.EmitCapabilityInterface()
        );
    }

    /// <summary>
    /// Emits the effects class with all effect methods.
    /// </summary>
    public static void EmitEffectsModule(this SourceProductionContext context, EffectsModuleModel module)
    {
        var hints = new HintNameProvider(module.Namespace);

        if (module.IsDbContext)
        {
            context.AddFromTemplate(
                Named("EffectsModuleDbContext.scriban-cs"),
                hints.Filename($"{module.Name}Effects"),
                module);
        }
        else
        {
            context.AddFromEmit(
                hints.Filename($"{module.Name}Effects"),
                module.EmitEffectsModule()
            );
        }
    }

    // ========== Shared Helpers ==========

    internal static TypeBuilder AddInstrumentationActivitySource(this TypeBuilder builder, EffectsModuleModel module) =>
        builder.If(module.Instrumented, b => b
            .AddUsing("System.Diagnostics")
            .AddField(FieldBuilder.Parse("public static readonly ActivitySource ActivitySource")
                .WithInitializer($"""new("{module.Namespace}.{module.Name}", "1.0.0")""")));
}
