using Deepstaging.Data.Models;
using Deepstaging.Roslyn.Emit;

namespace Deepstaging.Generators.Emitters;

/// <summary>
/// Emits code for standalone [EffectsModule] decorated classes.
/// </summary>
public static class EffectsModuleEmitter
{
    extension(EffectsModuleModel model)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public OptionalEmit EmitCapabilityInterface() => TypeBuilder
            .Interface(model.CapabilityInterface)
            .InNamespace(model.Namespace)
            .AddProperty(model.PropertyName, model.TargetType, builder => builder.AsReadOnly())
            .Emit();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
}

file static class DbContextEffectExtensions
{
    public static TypeBuilder AddDbContextEffects(this TypeBuilder builder, EffectsModuleModel model)
    {
        return builder.If(model.IsDbContext, b => b
            .AddUsings()
            .AddMethod(model.SaveChangesAsyncMethod().InstrumentMethod(model))
            .WithEach(model.DbSets, (container, dbSet) => container.AddNestedType(
                TypeBuilder.Parse($"public static partial class {dbSet.PropertyName}")
                    // Query builder
                    .AddMethod(model.DbSetQueryMethod(dbSet))
                    // Convenience query methods
                    .AddMethod(model.DbSetFindAsyncMethod(dbSet).InstrumentMethod(model, dbSet))
                    .AddMethod(model.DbSetToListAsyncMethod(dbSet).InstrumentMethod(model, dbSet))
                    .AddMethod(model.DbSetFirstOrNoneAsyncMethod(dbSet).InstrumentMethod(model, dbSet))
                    .AddMethod(model.DbSetCountAsyncMethod(dbSet).InstrumentMethod(model, dbSet))
                    .AddMethod(model.DbSetAnyAsyncMethod(dbSet).InstrumentMethod(model, dbSet))
                    // Modification methods
                    .AddMethod(model.DbSetAddMethod(dbSet).InstrumentMethod(model, dbSet))
                    .AddMethod(model.DbSetAddRangeMethod(dbSet).InstrumentMethod(model, dbSet))
                    .AddMethod(model.DbSetUpdateMethod(dbSet).InstrumentMethod(model, dbSet))
                    .AddMethod(model.DbSetRemoveMethod(dbSet).InstrumentMethod(model, dbSet))
                )
            )
        );
    }

    private static TypeBuilder AddUsings(this TypeBuilder builder) => builder
        .AddUsing("System.Diagnostics")
        .AddUsing("System.Linq.Expressions")
        .AddUsing("LanguageExt")
        .AddUsing("LanguageExt.Effects")
        .AddUsing("Microsoft.EntityFrameworkCore")
        .AddUsing("static LanguageExt.Prelude");

    private static MethodBuilder SaveChangesAsyncMethod(this EffectsModuleModel model) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, int> SaveChangesAsync<RT>(CancellationToken token = default) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Saves all changes made in this context to the database asynchronously.")
            .WithExpressionBody(
                $"""liftEff<RT, int>(async rt => await rt.{model.PropertyName}.SaveChangesAsync(token))"""
            );

    private static MethodBuilder InstrumentMethod(this MethodBuilder builder, EffectsModuleModel model) =>
        builder.If(model.Instrumented, _ => builder
            .AppendExpressionBody($""".WithActivity("{model.Name}.{builder.Name}", ActivitySource)"""));

    private static MethodBuilder InstrumentMethod(this MethodBuilder builder, EffectsModuleModel model, DbSetModel dbSet) =>
        builder.If(model.Instrumented, _ => builder
            .AppendExpressionBody($""".WithActivity("{model.Name}.{dbSet.PropertyName}.{builder.Name}", ActivitySource)"""));

    // ========== Query Methods ==========

    private static MethodBuilder DbSetQueryMethod(this EffectsModuleModel model, DbSetModel dbSet) =>
        MethodBuilder
            .Parse(
                $"public static DbSetQuery<RT, {dbSet.EntityType}> Query<RT>() where RT : {model.CapabilityInterface}")
            .WithXmlDoc("""
                        Returns a composable query builder for this entity set.
                        Chain LINQ methods and call a terminal operation (ToListAsync, FirstOrNoneAsync, etc.) to execute.
                        """)
            .WithExpressionBody($"new(rt => rt.{model.PropertyName}.{dbSet.PropertyName})");

    private static MethodBuilder DbSetFindAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, Option<{dbSet.EntityType}>> FindAsync<RT>(params object[] keyValues) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Finds an entity by its primary key values, returning None if not found.")
            .WithExpressionBody(
                $"""liftEff<RT, Option<{dbSet.EntityType}>>(async rt => Optional(await rt.{model.PropertyName}.{dbSet.PropertyName}.FindAsync(keyValues)))"""
            );

    private static MethodBuilder DbSetToListAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, List<{dbSet.EntityType}>> ToListAsync<RT>(CancellationToken token = default) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Returns all entities as a list.")
            .WithExpressionBody("Query<RT>().ToListAsync(token)");

    private static MethodBuilder DbSetFirstOrNoneAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, Option<{dbSet.EntityType}>> FirstOrNoneAsync<RT>(Expression<Func<{dbSet.EntityType}, bool>> predicate, CancellationToken token = default) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Returns the first entity matching the predicate, or None.")
            .WithExpressionBody("Query<RT>().FirstOrNoneAsync(predicate, token)");

    private static MethodBuilder DbSetCountAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, int> CountAsync<RT>(CancellationToken token = default) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Returns the count of entities.")
            .WithExpressionBody("Query<RT>().CountAsync(token)");

    private static MethodBuilder DbSetAnyAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, bool> AnyAsync<RT>(CancellationToken token = default) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Returns true if any entities exist.")
            .WithExpressionBody("Query<RT>().AnyAsync(token)");

    // ========== Modification Methods ==========

    private static MethodBuilder DbSetAddMethod(this EffectsModuleModel model, DbSetModel dbSet) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, Unit> Add<RT>({dbSet.EntityType} entity) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Begins tracking the entity in the Added state.")
            .WithExpressionBody(
                $$"""liftEff<RT, Unit>(rt => { rt.{{model.PropertyName}}.{{dbSet.PropertyName}}.Add(entity); return unit; })"""
            );

    private static MethodBuilder DbSetAddRangeMethod(this EffectsModuleModel model, DbSetModel dbSet) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, Unit> AddRange<RT>(params {dbSet.EntityType}[] entities) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Begins tracking the entities in the Added state.")
            .WithExpressionBody(
                $$"""liftEff<RT, Unit>(rt => { rt.{{model.PropertyName}}.{{dbSet.PropertyName}}.AddRange(entities); return unit; })"""
            );

    private static MethodBuilder DbSetUpdateMethod(this EffectsModuleModel model, DbSetModel dbSet) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, Unit> Update<RT>({dbSet.EntityType} entity) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Begins tracking the entity in the Modified state.")
            .WithExpressionBody(
                $$"""liftEff<RT, Unit>(rt => { rt.{{model.PropertyName}}.{{dbSet.PropertyName}}.Update(entity); return unit; })"""
            );

    private static MethodBuilder DbSetRemoveMethod(this EffectsModuleModel model, DbSetModel dbSet) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, Unit> Remove<RT>({dbSet.EntityType} entity) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Begins tracking the entity in the Deleted state.")
            .WithExpressionBody(
                $$"""liftEff<RT, Unit>(rt => { rt.{{model.PropertyName}}.{{dbSet.PropertyName}}.Remove(entity); return unit; })"""
            );
}

file static class EffectMethodExtensions
{
    public static TypeBuilder AddEffectMethods(this TypeBuilder builder, EffectsModuleModel module) =>
        builder.If(module.Methods.Any(), b => b
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

    public static TypeBuilder AddInstrumentationActivitySource(this TypeBuilder builder, EffectsModuleModel module) =>
        builder.If(module.Instrumented, b => b
            .AddUsing("System.Diagnostics")
            .AddField(FieldBuilder.Parse("public static readonly ActivitySource ActivitySource")
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