// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5


namespace Deepstaging.Generators.Writers.Effects;

using LocalRefs;
using static CollectionRefs;
using static LanguageExtRefs;

/// <summary>
/// DbContext-specific effect emissions (SaveChanges, DbSet methods).
/// </summary>
public static partial class EffectsModuleWriter
{
    private static TypeBuilder AddDbContextEffects(this TypeBuilder builder, EffectsModuleModel model) =>
        builder.If(model.IsDbContext, type => type
            .AddUsing(EntityFrameworkRefs.Namespace)
            .AddMethod(model.SaveChangesAsyncMethod().InstrumentMethod(model))
            .WithEach(model.DbSets, (container, dbSet) => container
                .AddNestedType(CreateDbSetModule(model, dbSet))));

    private static TypeBuilder CreateDbSetModule(EffectsModuleModel model, DbSetModel dbSet) =>
        TypeBuilder
            .Parse($"public static partial class {dbSet.PropertyName}")
            .WithXmlDoc($"Effect methods for the <c>{dbSet.PropertyName}</c> entity set.")
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
            .AddMethod(model.RemoveMethod(dbSet).InstrumentMethod(model, dbSet));

    // ========== Instrumentation ==========

    private static MethodBuilder InstrumentMethod(this MethodBuilder builder, EffectsModuleModel model) => builder
        .If(model.Instrumented, _ => builder
            .AppendExpressionBody($""".WithActivity("{model.Name}.{builder.Name}", ActivitySource)"""));

    private static MethodBuilder InstrumentMethod(this MethodBuilder builder, EffectsModuleModel model,
        DbSetModel dbSet) => builder
        .If(model.Instrumented, _ => builder
            .AppendExpressionBody($""".WithActivity("{model.Name}.{dbSet.PropertyName}.{builder.Name}", ActivitySource)"""));

    // ========== DbContext Methods ==========

    private static MethodBuilder SaveChangesAsyncMethod(this EffectsModuleModel model) => MethodBuilder
        .Parse(
            $"""
             public static {EffRT.Of("int")} SaveChangesAsync<RT>(
                {TaskRefs.CancellationToken} token = default
             ) where RT : {model.Capability.Interface} 
             """)
        .WithXmlDoc(xml => xml
            .WithSummary("Saves all changes made in this context to the database asynchronously.")
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
            .WithReturns("An effect that yields the number of state entries written to the database."))
        .WithExpressionBody(EffExpression.Lift("RT", "rt")
            .Async("int", $"rt.{model.Capability.PropertyName}.SaveChangesAsync(token)"));

    // ========== DbSet Query Methods ==========

    private static MethodBuilder QueryMethod(this EffectsModuleModel model, DbSetModel dbSet) => MethodBuilder
        .Parse(
            $"""
             public static {DbSetRefs.DbSetQueryOf(dbSet.EntityType)} Query<RT>() 
                where RT : {model.Capability.Interface}
             """)
        .WithXmlDoc(xml => xml
            .WithSummary(
                """
                Returns a composable query builder for this entity set.
                Chain LINQ methods and call a terminal operation (ToListAsync, FirstOrNoneAsync, etc.) to execute.
                """)
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .WithReturns("A composable query builder for the entity set."))
        .WithExpressionBody($"new(rt => rt.{model.Capability.PropertyName}.{dbSet.PropertyName})");

    private static MethodBuilder FindAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet) => MethodBuilder
        .Parse(
            $"""
             public static {EffRT.Of(Option(dbSet.EntityType))} FindAsync<RT>(
                params object[] keyValues
             ) where RT : {model.Capability.Interface}
             """)
        .WithXmlDoc(xml => xml
            .WithSummary("Finds an entity by its primary key values, returning None if not found.")
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .AddParam("keyValues", "The primary key values of the entity to find.")
            .WithReturns("An effect that yields the entity wrapped in an Option, or None if not found."))
        .WithExpressionBody(EffExpression.Lift("RT", "rt")
            .AsyncOptional(Option(dbSet.EntityType), $"rt.{model.Capability.PropertyName}.{dbSet.PropertyName}.FindAsync(keyValues)"));

    private static MethodBuilder ToListAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet) => MethodBuilder
        .Parse(
            $"""
             public static {EffRT.Of(List(dbSet.EntityType))} ToListAsync<RT>(
                {TaskRefs.CancellationToken} token = default
             ) where RT : {model.Capability.Interface}
             """)
        .WithXmlDoc(xml => xml
            .WithSummary("Returns all entities as a list.")
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
            .WithReturns("An effect that yields a list of all entities."))
        .WithExpressionBody("Query<RT>().ToListAsync(token)");

    private static MethodBuilder FirstOrNoneAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet) => MethodBuilder
        .Parse(
            $"""
             public static {EffRT.Of(Option(dbSet.EntityType))} FirstOrNoneAsync<RT>(
                {ExpressionsRefs.Expression(dbSet.EntityType, "bool")} predicate,
                {TaskRefs.CancellationToken} token = default
             ) where RT : {model.Capability.Interface}
             """)
        .WithXmlDoc(xml => xml
            .WithSummary("Returns the first entity matching the predicate, or None.")
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .AddParam("predicate", "The LINQ expression representing the filter condition.")
            .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
            .WithReturns("An effect that yields the matching entity wrapped in an Option, or None if not found."))
        .WithExpressionBody("Query<RT>().FirstOrNoneAsync(predicate, token)");

    private static MethodBuilder CountAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet) => MethodBuilder
        .Parse(
            $"""
             public static {EffRT.Of("int")} CountAsync<RT>(
                {TaskRefs.CancellationToken} token = default
             ) where RT : {model.Capability.Interface}
             """)
        .WithXmlDoc(xml => xml
            .WithSummary("Returns the count of entities.")
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
            .WithReturns("An effect that yields the number of entities."))
        .WithExpressionBody("Query<RT>().CountAsync(token)");

    private static MethodBuilder AnyAsyncMethod(this EffectsModuleModel model, DbSetModel dbSet) => MethodBuilder
        .Parse(
            $"""
             public static {EffRT.Of("bool")} AnyAsync<RT>(
                {TaskRefs.CancellationToken} token = default
             ) where RT : {model.Capability.Interface}
             """)
        .WithXmlDoc(xml => xml
            .WithSummary("Returns true if any entities exist.")
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
            .WithReturns("An effect that yields <see langword=\"true\" /> if the entity set contains any elements."))
        .WithExpressionBody("Query<RT>().AnyAsync(token)");

    // ========== DbSet Modification Methods ==========

    private static MethodBuilder AddMethod(this EffectsModuleModel model, DbSetModel dbSet) => MethodBuilder
        .Parse(
            $"""
             public static {EffRT.Of(dbSet.EntityType)} Add<RT>(
                {dbSet.EntityType} entity
             ) where RT : {model.Capability.Interface}
             """)
        .WithXmlDoc(xml => xml
            .WithSummary("Begins tracking the entity in the Added state so it will be inserted on SaveChanges.")
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .AddParam("entity", "The entity to add.")
            .WithReturns("An effect that yields the tracked entity."))
        .WithExpressionBody(EffExpression.Lift("RT", "rt")
            .Body(dbSet.EntityType, $"rt => {{ rt.{model.Capability.PropertyName}.{dbSet.PropertyName}.Add(entity); return entity; }}"));

    private static MethodBuilder AddRangeMethod(this EffectsModuleModel model, DbSetModel dbSet) => MethodBuilder
        .Parse(
            $"""
             public static {EffRT.Array(dbSet.EntityType)} AddRange<RT>(
                params {dbSet.EntityType.Array()} entities
             ) where RT : {model.Capability.Interface}
             """)
        .WithExpressionBody(EffExpression.Lift("RT", "rt")
            .Body(dbSet.EntityType.Array(), $"rt => {{ rt.{model.Capability.PropertyName}.{dbSet.PropertyName}.AddRange(entities); return entities; }}"))
        .WithXmlDoc(xml => xml
            .WithSummary("Begins tracking the entities in the Added state so they will be inserted on SaveChanges.")
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .AddParam("entities", "The entities to add.")
            .WithReturns("An effect that yields the tracked entities."));

    private static MethodBuilder UpdateMethod(this EffectsModuleModel model, DbSetModel dbSet) => MethodBuilder
        .Parse(
            $"""
             public static {EffRT.Of(dbSet.EntityType)} Update<RT>(
                {dbSet.EntityType} entity
             ) where RT : {model.Capability.Interface}
             """)
        .WithExpressionBody(EffExpression.Lift("RT", "rt")
            .Body(dbSet.EntityType, $"rt => {{ rt.{model.Capability.PropertyName}.{dbSet.PropertyName}.Update(entity); return entity; }}"))
        .WithXmlDoc(xml => xml
            .WithSummary("Begins tracking the entity in the Modified state so it will be updated on SaveChanges.")
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .AddParam("entity", "The entity to update.")
            .WithReturns("An effect that yields the tracked entity."));

    private static MethodBuilder RemoveMethod(this EffectsModuleModel model, DbSetModel dbSet) => MethodBuilder
        .Parse(
            $"""
             public static {EffRT.Of(dbSet.EntityType)} Remove<RT>(
                {dbSet.EntityType} entity
             ) where RT : {model.Capability.Interface}
             """)
        .WithExpressionBody(EffExpression.Lift("RT", "rt")
            .Body(dbSet.EntityType, $"rt => {{ rt.{model.Capability.PropertyName}.{dbSet.PropertyName}.Remove(entity); return entity; }}"))
        .WithXmlDoc(xml => xml
            .WithSummary("Begins tracking the entity in the Deleted state so it will be removed on SaveChanges.")
            .AddTypeParam("RT", "The runtime type providing database context access.")
            .AddParam("entity", "The entity to remove.")
            .WithReturns("An effect that yields the removed entity."));
}