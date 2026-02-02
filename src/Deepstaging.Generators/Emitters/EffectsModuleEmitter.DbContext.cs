using Deepstaging.Data.Models;
using Deepstaging.Roslyn.Emit;

namespace Deepstaging.Generators.Emitters;

/// <summary>
/// DbContext-specific effect emissions (SaveChanges, DbSet methods).
/// </summary>
public static partial class EffectsModuleEmitter
{
    internal static TypeBuilder AddDbContextEffects(this TypeBuilder builder, EffectsModuleModel model)
    {
        return builder.If(model.IsDbContext, b => b
            .AddDbContextUsings()
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

    private static TypeBuilder AddDbContextUsings(this TypeBuilder builder) => builder
        .AddUsing("System.Diagnostics")
        .AddUsing("System.Linq.Expressions")
        .AddUsing("LanguageExt")
        .AddUsing("LanguageExt.Effects")
        .AddUsing("Microsoft.EntityFrameworkCore")
        .AddUsing("static LanguageExt.Prelude");

    // ========== Instrumentation ==========

    private static MethodBuilder InstrumentMethod(this MethodBuilder builder, EffectsModuleModel model) =>
        builder.If(model.Instrumented, _ => builder
            .AppendExpressionBody($""".WithActivity("{model.Name}.{builder.Name}", ActivitySource)"""));

    private static MethodBuilder InstrumentMethod(this MethodBuilder builder, EffectsModuleModel model, DbSetModel dbSet) =>
        builder.If(model.Instrumented, _ => builder
            .AppendExpressionBody($""".WithActivity("{model.Name}.{dbSet.PropertyName}.{builder.Name}", ActivitySource)"""));

    // ========== DbContext Methods ==========

    private static MethodBuilder SaveChangesAsyncMethod(this EffectsModuleModel model) =>
        MethodBuilder
            .Parse(
                $"public static Eff<RT, int> SaveChangesAsync<RT>(CancellationToken token = default) where RT : {model.CapabilityInterface}")
            .WithXmlDoc("Saves all changes made in this context to the database asynchronously.")
            .WithExpressionBody(
                $"""liftEff<RT, int>(async rt => await rt.{model.PropertyName}.SaveChangesAsync(token))"""
            );

    // ========== DbSet Query Methods ==========

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

    // ========== DbSet Modification Methods ==========

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
