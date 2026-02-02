using Deepstaging.Data.Models;
using Deepstaging.Roslyn.Emit;

namespace Deepstaging.Generators.Emitters;

/// <summary>
/// Emits code for standalone [EffectsModule] decorated classes.
/// </summary>
public static class EffectsModuleEmitter
{
    private static readonly Func<string, TemplateName> Named =
        TemplateName.ForGenerator<DeepstagingGenerator>();

    /// <summary>
    /// Emits the capability interface for a standalone effects module.
    /// </summary>
    public static void EmitCapabilityInterface(this SourceProductionContext context, EffectsModuleModel module)
    {
        context.AddFromEmit(
            hintName: new HintNameProvider(module.Namespace)
                .Filename(module.CapabilityInterface),
            emit: TypeBuilder
                .Interface(module.CapabilityInterface)
                .InNamespace(module.Namespace)
                .AddProperty(module.PropertyName, module.TargetType, builder => builder.AsReadOnly())
                .Emit()
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
                module.GenerateEffectsModule()
            );
        }
    }
}

file static class BuilderExtensions
{
    public static OptionalEmit GenerateEffectsModule(this EffectsModuleModel module) =>
        TypeBuilder
            .Class($"{module.Name}Effects").AsPartial().AsStatic()
            .InNamespace(module.Namespace)
            .AddUsing("Deepstaging.Effects")
            .AddInstrumentationActivitySource(module)
            .AddEffectMethods(module)
            .Emit();

    private static TypeBuilder AddEffectMethods(this TypeBuilder builder, EffectsModuleModel module) =>
        builder.IfNot(module.Methods.IsDefaultOrEmpty, b => b
                .AddUsing("LanguageExt")
                .AddUsing("LanguageExt.Effects")
                .AddUsing("static LanguageExt.Prelude"))
            .WithEach(module.Methods, (b, method) =>
                b.AddMethod(method.EffectName, m => m
                    .AsStatic()
                    .AddTypeParameter("RT", tp => tp.WithConstraint(module.CapabilityInterface))
                    .AddMethodParameters(method)
                    .WithReturnType($"Eff<RT, {method.EffResultType}>")
                    .WithXmlDoc(method.XmlDocumentation)
                    .WithExpressionBody(module.LiftedMethodExpression(method))));

    private static MethodBuilder AddMethodParameters(this MethodBuilder builder, EffectMethodModel method) =>
        builder.WithEach(method.Parameters, (b, param) => param.HasDefaultValue
            ? b.AddParameter(param.Name, param.Type, p => p.WithDefaultValue(param.DefaultValue!))
            : b.AddParameter(param.Name, param.Type));

    private static TypeBuilder AddInstrumentationActivitySource(this TypeBuilder builder, EffectsModuleModel module) =>
        builder.If(module.Instrumented, b => b
            .AddUsing("System.Diagnostics")
            .AddField("ActivitySource", "ActivitySource", field => field
                .AsStatic()
                .AsReadonly()
                .WithInitializer($"""new("{module.Namespace}.{module.Name}", "1.0.0")""")));

    private static string LiftedMethodExpression(this EffectsModuleModel module, EffectMethodModel method)
    {
        var paramList = string.Join(", ", method.Parameters.Select(p => p.Name));
        var methodCall = $"rt.{module.PropertyName}.{method.SourceMethodName}({paramList})";

        var expression = method.LiftingStrategy switch
        {
            // liftEff<RT, T>(async rt => await rt.Prop.Method(params))
            EffectLiftingStrategy.AsyncValue =>
                $"liftEff<RT, {method.EffResultType}>(async rt => await {methodCall})",

            // liftEff<RT, Unit>(async rt => { await rt.Prop.Method(params); return unit; })
            EffectLiftingStrategy.AsyncVoid =>
                $$"""liftEff<RT, Unit>(async rt => { await {{methodCall}}; return unit; })""",

            // liftEff<RT, Option<T>>(async rt => Optional(await rt.Prop.Method(params)))
            EffectLiftingStrategy.AsyncNullableToOption =>
                $"liftEff<RT, {method.EffResultType}>(async rt => Optional(await {methodCall}))",

            // liftEff<RT, T>(rt => rt.Prop.Method(params))
            EffectLiftingStrategy.SyncValue =>
                $"liftEff<RT, {method.EffResultType}>(rt => {methodCall})",

            // liftEff<RT, Unit>(rt => { rt.Prop.Method(params); return unit; })
            EffectLiftingStrategy.SyncVoid =>
                $$"""liftEff<RT, Unit>(rt => { {{methodCall}}; return unit; })""",

            // liftEff<RT, Option<T>>(rt => Optional(rt.Prop.Method(params)))
            EffectLiftingStrategy.SyncNullableToOption =>
                $"liftEff<RT, {method.EffResultType}>(rt => Optional({methodCall}))",

            _ => throw new ArgumentOutOfRangeException(nameof(method.LiftingStrategy))
        };

        return module.Instrumented
            ? $"{expression}.WithActivity(\"{module.Name}.{method.EffectName}\", ActivitySource)"
            : expression;
    }
}