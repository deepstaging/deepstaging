namespace Deepstaging.Generators.Writers;

/// <summary>
/// DbContext-specific effect emissions (SaveChanges, DbSet methods).
/// </summary>
public static partial class EffectsModuleWriter
{
    extension(TypeBuilder builder)
    {
        private TypeBuilder AddDbContextEffects(EffectsModuleModel model)
        {
            return builder.If(model.IsDbContext, b => b
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddMethod(model.SaveChangesAsyncMethod().InstrumentMethod(model))
                .WithEach(model.DbSets, (container, dbSet) =>
                    container.AddNestedType(
                        TypeBuilder.Parse($"public static partial class {dbSet.PropertyName}")
                            // Query builder
                            .AddMethod(model.QueryMethod(dbSet))
                            // Convenience query methods
                            .AddMethod(model.FindAsyncMethod(dbSet).InstrumentMethod(model, dbSet))
                            .AddMethod(model.ToListAsyncMethod(dbSet).InstrumentMethod(model, dbSet))
                            .AddMethod(model.FirstOrNoneAsyncMethod(dbSet).InstrumentMethod(model, dbSet))
                            .AddMethod(model.CountAsyncMethod(dbSet).InstrumentMethod(model, dbSet))
                            .AddMethod(model.AnyAsyncMethod(dbSet).InstrumentMethod(model, dbSet))
                            // Modification methods
                            .AddMethod(model.AddMethod(dbSet).InstrumentMethod(model, dbSet))
                            .AddMethod(model.AddRangeMethod(dbSet).InstrumentMethod(model, dbSet))
                            .AddMethod(model.UpdateMethod(dbSet).InstrumentMethod(model, dbSet))
                            .AddMethod(model.RemoveMethod(dbSet).InstrumentMethod(model, dbSet)))
                )
            );
        }
    }

    // ========== Instrumentation ==========

    private static MethodBuilder InstrumentMethod(this MethodBuilder builder, EffectsModuleModel model)
    {
        return builder.If(model.Instrumented, _ => builder
            .AppendExpressionBody($""".WithActivity("{model.Name}.{builder.Name}", ActivitySource)"""));
    }

    private static MethodBuilder InstrumentMethod(this MethodBuilder builder, EffectsModuleModel model,
        DbSetModel dbSet)
    {
        return builder.If(model.Instrumented, _ => builder
            .AppendExpressionBody(
                $""".WithActivity("{model.Name}.{dbSet.PropertyName}.{builder.Name}", ActivitySource)"""));
    }

    // ========== DbContext Methods ==========

    private static MethodBuilder SaveChangesAsyncMethod(this EffectsModuleModel model)
    {
        return MethodBuilder
            .Parse(
                $"public static Eff<RT, int> SaveChangesAsync<RT>(CancellationToken token = default) where RT : {model.Capability.Interface}")
            .WithXmlDoc("Saves all changes made in this context to the database asynchronously.")
            .WithExpressionBody(
                $"""liftEff<RT, int>(async rt => await rt.{model.Capability.PropertyName}.SaveChangesAsync(token))"""
            );
    }

    // ========== DbSet Query Methods ==========

    private static MethodBuilder QueryMethod(this EffectsModuleModel model, DbSetModel dbSet)
    {
        return MethodBuilder
            .Parse(
                $"public static DbSetQuery<RT, {dbSet.EntityType}> Query<RT>() where RT : {model.Capability.Interface}")
            .WithXmlDoc("""
                        Returns a composable query builder for this entity set.
                        Chain LINQ methods and call a terminal operation (ToListAsync, FirstOrNoneAsync, etc.) to execute.
                        """)
            .WithExpressionBody($"new(rt => rt.{model.Capability.PropertyName}.{dbSet.PropertyName})");
    }

    private static MethodBuilder FindAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet)
    {
        return MethodBuilder
            .Parse(
                $"public static Eff<RT, Option<{dbSet.EntityType}>> FindAsync<RT>(params object[] keyValues) where RT : {model.Capability.Interface}")
            .WithXmlDoc("Finds an entity by its primary key values, returning None if not found.")
            .WithExpressionBody(
                $"""liftEff<RT, Option<{dbSet.EntityType}>>(async rt => Optional(await rt.{model.Capability.PropertyName}.{dbSet.PropertyName}.FindAsync(keyValues)))"""
            );
    }

    private static MethodBuilder ToListAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet)
    {
        return MethodBuilder
            .Parse(
                $"public static Eff<RT, List<{dbSet.EntityType}>> ToListAsync<RT>(CancellationToken token = default) where RT : {model.Capability.Interface}")
            .WithXmlDoc("Returns all entities as a list.")
            .WithExpressionBody("Query<RT>().ToListAsync(token)");
    }

    private static MethodBuilder FirstOrNoneAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet)
    {
        return MethodBuilder
            .Parse(
                $"public static Eff<RT, Option<{dbSet.EntityType}>> FirstOrNoneAsync<RT>(Expression<Func<{dbSet.EntityType}, bool>> predicate, CancellationToken token = default) where RT : {model.Capability.Interface}")
            .WithXmlDoc("Returns the first entity matching the predicate, or None.")
            .WithExpressionBody("Query<RT>().FirstOrNoneAsync(predicate, token)");
    }

    private static MethodBuilder CountAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet)
    {
        return MethodBuilder
            .Parse(
                $"public static Eff<RT, int> CountAsync<RT>(CancellationToken token = default) where RT : {model.Capability.Interface}")
            .WithXmlDoc("Returns the count of entities.")
            .WithExpressionBody("Query<RT>().CountAsync(token)");
    }

    private static MethodBuilder AnyAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet)
    {
        return MethodBuilder
            .Parse(
                $"public static Eff<RT, bool> AnyAsync<RT>(CancellationToken token = default) where RT : {model.Capability.Interface}")
            .WithXmlDoc("Returns true if any entities exist.")
            .WithExpressionBody("Query<RT>().AnyAsync(token)");
    }

    // ========== DbSet Modification Methods ==========

    private static MethodBuilder AddMethod(this EffectsModuleModel model, DbSetModel dbSet)
    {
        return MethodBuilder
            .Parse(
                $"public static Eff<RT, Unit> Add<RT>({dbSet.EntityType} entity) where RT : {model.Capability.Interface}")
            .WithXmlDoc("Begins tracking the entity in the Added state.")
            .WithExpressionBody(
                $$"""liftEff<RT, Unit>(rt => { rt.{{model.Capability.PropertyName}}.{{dbSet.PropertyName}}.Add(entity); return unit; })"""
            );
    }

    private static MethodBuilder AddRangeMethod(this EffectsModuleModel model, DbSetModel dbSet)
    {
        return MethodBuilder
            .Parse(
                $"public static Eff<RT, Unit> AddRange<RT>(params {dbSet.EntityType}[] entities) where RT : {model.Capability.Interface}")
            .WithXmlDoc("Begins tracking the entities in the Added state.")
            .WithExpressionBody(
                $$"""liftEff<RT, Unit>(rt => { rt.{{model.Capability.PropertyName}}.{{dbSet.PropertyName}}.AddRange(entities); return unit; })"""
            );
    }

    private static MethodBuilder UpdateMethod(this EffectsModuleModel model, DbSetModel dbSet)
    {
        return MethodBuilder
            .Parse(
                $"public static Eff<RT, Unit> Update<RT>({dbSet.EntityType} entity) where RT : {model.Capability.Interface}")
            .WithXmlDoc("Begins tracking the entity in the Modified state.")
            .WithExpressionBody(
                $$"""liftEff<RT, Unit>(rt => { rt.{{model.Capability.PropertyName}}.{{dbSet.PropertyName}}.Update(entity); return unit; })"""
            );
    }

    private static MethodBuilder RemoveMethod(this EffectsModuleModel model, DbSetModel dbSet)
    {
        return MethodBuilder
            .Parse(
                $"public static Eff<RT, Unit> Remove<RT>({dbSet.EntityType} entity) where RT : {model.Capability.Interface}")
            .WithXmlDoc("Begins tracking the entity in the Deleted state.")
            .WithExpressionBody(
                $$"""liftEff<RT, Unit>(rt => { rt.{{model.Capability.PropertyName}}.{{dbSet.PropertyName}}.Remove(entity); return unit; })"""
            );
    }
}