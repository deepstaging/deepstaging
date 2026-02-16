// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Effects;

using LocalRefs;

/// <summary>
/// Emits the <c>DbSetQuery</c> helper class with a nested <c>DbSetOrderedQuery</c> type.
/// </summary>
public static class DbSetQueryWriter
{
    extension(ImmutableArray<EffectsModuleModel> _)
    {
        /// <summary>
        /// Emits the <c>DbSetQuery</c> helper class with a nested <c>DbSetOrderedQuery</c> type.
        /// </summary>
        /// <returns>The emitted code, or an error if emission failed.</returns>
        public OptionalEmit WriteDbSetQueryHelper() =>
            TypeBuilder
                .Parse($"public sealed class {DbSetRefs.DbSetQueryType} where T : class")
                .InNamespace("Deepstaging.Effects")
                .AddUsings(LinqRefs.Namespace, LinqRefs.ExpressionsNamespace)
                .AddUsings(LanguageExtRefs.Namespace, LanguageExtRefs.PreludeStatic)
                .AddUsings(TaskRefs.ThreadingNamespace, "Microsoft.EntityFrameworkCore")
                .WithXmlDoc(xml => xml
                    .WithSummary("A composable query builder for DbSet that accumulates LINQ expressions and materializes to an Eff only on terminal operations.")
                    .AddTypeParam("RT", "The runtime type that provides the DbContext capability.")
                    .AddTypeParam("T", "The entity type of the DbSet being queried.")
                )
                .WithPrimaryConstructor(c => c.AddParameter("query", DbSetRefs.DbSetQueryFactory))
                .AddFilteringMethods()
                .AddOrderingMethods()
                .AddEagerLoadingMethods()
                .AddPaginationMethods()
                .AddProjectionMethods()
                .AddSetOperationMethods()
                .AddTrackingMethods()
                .AddTerminalEffectMethods()
                .AddAggregateMethods()
                .AddNestedType(BuildOrderedQueryType())
                .Emit();
    }

    // ========== DbSetQuery Methods ==========

    private static TypeBuilder AddFilteringMethods(this TypeBuilder builder) => builder
        .AddRegion("Filtering Operations", type => type
            .AddMethod("Where", m => m
                .AddParameter("predicate", ExpressionsRefs.Predicate)
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithExpressionBody(DbSetRefs.DbSetQueryType.New("rt => query(rt).Where(predicate)"))
                .WithXmlDoc(xml => xml
                    .WithSummary("Filters the sequence based on a predicate.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .WithReturns("A new query instance with the added filter."))));

    private static TypeBuilder AddOrderingMethods(this TypeBuilder builder) => builder
        .AddRegion("Ordering Operations", type => type
            .AddMethod("OrderBy", m => m
                .AddTypeParameter("TKey")
                .AddParameter("keySelector", ExpressionsRefs.Expression("TKey"))
                .WithReturnType(DbSetRefs.OrderedDbSetQueryType)
                .WithExpressionBody($"new {DbSetRefs.OrderedDbSetQueryType}(rt => query(rt).OrderBy(keySelector))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Sorts the elements in ascending order.")
                    .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                    .WithReturns("A new ordered query instance.")))
            .AddMethod("OrderByDescending", m => m
                .AddTypeParameter("TKey")
                .AddParameter("keySelector", ExpressionsRefs.Expression("TKey"))
                .WithReturnType(DbSetRefs.OrderedDbSetQueryType)
                .WithExpressionBody($"new {DbSetRefs.OrderedDbSetQueryType}(rt => query(rt).OrderByDescending(keySelector))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Sorts the elements in descending order.")
                    .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                    .WithReturns("A new ordered query instance."))));

    private static TypeBuilder AddEagerLoadingMethods(this TypeBuilder builder) => builder
        .AddRegion("Eager Loading Operations", type => type
            .AddMethod("Include", m => m
                .AddTypeParameter("TProperty")
                .AddParameter("navigationPropertyPath", ExpressionsRefs.Expression("TProperty"))
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryType}(rt => query(rt).Include(navigationPropertyPath))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Includes a related entity in the query results.")
                    .AddParam("navigationPropertyPath", "The navigation property to include.")
                    .WithReturns("A new query instance with the eager loading applied.")
                ))
            .AddMethod("Include", m => m
                .AddParameter("navigationPropertyPath", "string")
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryType}(rt => query(rt).Include(navigationPropertyPath))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Includes a related entity using a string path.")
                    .AddParam("navigationPropertyPath", "The string navigation property path to include.")
                    .WithReturns("A new query instance with the eager loading applied.")
                )));

    private static TypeBuilder AddPaginationMethods(this TypeBuilder builder) => builder
        .AddRegion("Pagination Operations", type => type
            .AddMethod("Skip", m => m
                .AddParameter("count", "int")
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryType}(rt => query(rt).Skip(count))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Bypasses a specified number of elements.")
                    .AddParam("count", "The number of elements to skip.")
                    .WithReturns("A new query instance with the skip applied.")
                )
            )
            .AddMethod("Take", m => m
                .AddParameter("count", "int")
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryType}(rt => query(rt).Take(count))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns a specified number of elements.")
                    .AddParam("count", "The number of elements to return.")
                    .WithReturns("A new query instance with the take applied.")
                )));

    // TODO: Finish reformatting Select and SelectMany — complex generic signatures
    private static TypeBuilder AddProjectionMethods(this TypeBuilder builder) => builder
        .AddRegion("Projection Operations", type => type
            .AddMethod(MethodBuilder
                .Parse($"public {DbSetRefs.DbSetQueryOf("TResult")} Select<TResult>({ExpressionsRefs.Expression("TResult")} selector) where TResult : class")
                .WithXmlDoc(xml => xml
                    .WithSummary("Projects each element to a new form.")
                    .AddParam("selector", "The projection expression.")
                    .WithReturns("A new query instance projecting to the result type."))
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryOf("TResult")}(rt => query(rt).Select(selector))"))
            .AddMethod(MethodBuilder
                .Parse(
                    $"public {DbSetRefs.DbSetQueryOf("TResult")} SelectMany<TResult>({ExpressionsRefs.Expression(CollectionRefs.IEnumerable("TResult"))} selector) where TResult : class")
                .WithXmlDoc(xml => xml
                    .WithSummary("Projects and flattens sequences.")
                    .AddParam("selector", "The projection expression.")
                    .WithReturns("A new query instance projecting and flattening to the result type."))
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryOf("TResult")}(rt => query(rt).SelectMany(selector))")));

    private static TypeBuilder AddSetOperationMethods(this TypeBuilder builder) => builder
        .AddRegion("Set Operations", type => type
            .AddMethod("Distinct", m => m
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns distinct elements.")
                    .WithReturns("A new query instance with distinct applied."))
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryType}(rt => Queryable.Distinct(query(rt)))")));

    private static TypeBuilder AddTrackingMethods(this TypeBuilder builder) => builder
        .AddRegion("Tracking Operations", type => type
            .AddMethod("AsNoTracking", m => m
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Disables change tracking for the query.")
                    .WithReturns("A new query instance with no tracking."))
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryType}(rt => query(rt).AsNoTracking())"))
            .AddMethod("AsNoTrackingWithIdentityResolution", m => m
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Disables change tracking but enables identity resolution.")
                    .WithReturns("A new query instance with identity resolution tracking."))
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryType}(rt => query(rt).AsNoTrackingWithIdentityResolution())"))
            .AddMethod("AsTracking", m => m
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Enables change tracking.")
                    .WithReturns("A new query instance with tracking enabled."))
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryType}(rt => query(rt).AsTracking())")));

    private static TypeBuilder AddTerminalEffectMethods(this TypeBuilder builder) => builder
        .AddRegion("Terminal Effects", type => type
            .AddCommonTerminalMethods()
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.OptionT} FirstOrNoneAsync({ExpressionsRefs.Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the first element matching the predicate, or None if not found.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the matching element wrapped in an Option, or None."))
                .WithExpressionBody($"{EffRefs.OptionT}.LiftIO(async rt => Optional(await query(rt).FirstOrDefaultAsync(predicate, token)))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.OptionT} SingleOrNoneAsync(CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the single element, or None if empty. Throws if more than one element exists.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the single element wrapped in an Option, or None."))
                .WithExpressionBody($"{EffRefs.OptionT}.LiftIO(async rt => Optional(await query(rt).SingleOrDefaultAsync(token)))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.OptionT} SingleOrNoneAsync({ExpressionsRefs.Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the single element matching the predicate, or None. Throws if more than one element matches.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the matching element wrapped in an Option, or None."))
                .WithExpressionBody($"{EffRefs.OptionT}.LiftIO(async rt => Optional(await query(rt).SingleOrDefaultAsync(predicate, token)))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.Int} CountAsync(CancellationToken token = default)")
                .WithExpressionBody($"{EffRefs.Int}.LiftIO(async rt => await query(rt).CountAsync(token))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the count of elements in the sequence.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the number of elements.")))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.Int} CountAsync({ExpressionsRefs.Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the count of elements matching the predicate.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the number of matching elements."))
                .WithExpressionBody($"{EffRefs.Int}.LiftIO(async rt => await query(rt).CountAsync(predicate, token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.Long} LongCountAsync(CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the count of elements as a 64-bit integer.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the number of elements as a <see langword=\"long\" />."))
                .WithExpressionBody($"{EffRefs.Long}.LiftIO(async rt => await query(rt).LongCountAsync(token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.Bool} AnyAsync(CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Determines whether the sequence contains any elements.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields <see langword=\"true\" /> if the sequence contains any elements."))
                .WithExpressionBody($"{EffRefs.Bool}.LiftIO(async rt => await query(rt).AnyAsync(token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.Bool} AnyAsync({ExpressionsRefs.Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Determines whether any element satisfies the predicate.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields <see langword=\"true\" /> if any element matches the predicate."))
                .WithExpressionBody($"{EffRefs.Bool}.LiftIO(async rt => await query(rt).AnyAsync(predicate, token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.Bool} AllAsync({ExpressionsRefs.Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Determines whether all elements satisfy the predicate.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields <see langword=\"true\" /> if every element matches the predicate."))
                .WithExpressionBody($"{EffRefs.Bool}.LiftIO(async rt => await query(rt).AllAsync(predicate, token))")));

    private static TypeBuilder AddAggregateMethods(this TypeBuilder builder) => builder
        .AddRegion("Aggregate Operations", type => type
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.T} MaxAsync(CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the maximum value in the sequence.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the maximum value."))
                .WithExpressionBody($"{EffRefs.T}.LiftIO(async rt => (await query(rt).MaxAsync(token))!)"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.TResult} MaxAsync<TResult>({ExpressionsRefs.Expression("TResult")} selector, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the maximum value of the selected projection.")
                    .AddTypeParam("TResult", "The type of the projected value.")
                    .AddParam("selector", "The projection expression to apply to each element.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the maximum projected value."))
                .WithExpressionBody($"{EffRefs.TResult}.LiftIO(async rt => (await query(rt).MaxAsync(selector, token))!)"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.T} MinAsync(CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the minimum value in the sequence.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the minimum value."))
                .WithExpressionBody($"{EffRefs.T}.LiftIO(async rt => (await query(rt).MinAsync(token))!)"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffRefs.TResult} MinAsync<TResult>({ExpressionsRefs.Expression("TResult")} selector, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the minimum value of the selected projection.")
                    .AddTypeParam("TResult", "The type of the projected value.")
                    .AddParam("selector", "The projection expression to apply to each element.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the minimum projected value."))
                .WithExpressionBody($"{EffRefs.TResult}.LiftIO(async rt => (await query(rt).MinAsync(selector, token))!)")));

    // ========== Shared terminal methods (used by both DbSetQuery and DbSetOrderedQuery) ==========

    private static TypeBuilder AddCommonTerminalMethods(this TypeBuilder builder) => builder
        .AddMethod(MethodBuilder
            .Parse($"public {EffRefs.ListT} ToListAsync(CancellationToken token = default)")
            .WithXmlDoc(xml => xml
                .WithSummary("Executes the query and returns all results as a list.")
                .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                .WithReturns("An effect that yields a list of all matching elements."))
            .WithExpressionBody($"{EffRefs.ListT}.LiftIO(async rt => await query(rt).ToListAsync(token))"))
        .AddMethod(MethodBuilder
            .Parse($"public {EffRefs.ArrayT} ToArrayAsync(CancellationToken token = default)")
            .WithXmlDoc(xml => xml
                .WithSummary("Executes the query and returns all results as an array.")
                .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                .WithReturns("An effect that yields an array of all matching elements."))
            .WithExpressionBody($"{EffRefs.ArrayT}.LiftIO(async rt => await query(rt).ToArrayAsync(token))"))
        .AddMethod(MethodBuilder
            .Parse($"public {EffRefs.OptionT} FirstOrNoneAsync(CancellationToken token = default)")
            .WithXmlDoc(xml => xml
                .WithSummary("Returns the first element, or None if the sequence is empty.")
                .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                .WithReturns("An effect that yields the first element wrapped in an Option, or None."))
            .WithExpressionBody($"{EffRefs.OptionT}.LiftIO(async rt => Optional(await query(rt).FirstOrDefaultAsync(token)))"));

    // ========== Nested DbSetOrderedQuery ==========

    private static TypeBuilder BuildOrderedQueryType() => TypeBuilder
        .Parse("public sealed class DbSetOrderedQuery")
        .WithXmlDoc("An ordered query builder that supports ThenBy operations.")
        .WithPrimaryConstructor(c => c.AddParameter("query", DbSetRefs.OrderedDbSetQueryFactory))
        .AddRegion("Ordering Operations", type => type
            .AddMethod("ThenBy", m => m
                .AddTypeParameter("TKey")
                .AddParameter("keySelector", ExpressionsRefs.Expression("TKey"))
                .WithReturnType(DbSetRefs.OrderedDbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Performs a subsequent ordering in ascending order.")
                    .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                    .WithReturns("A new ordered query instance."))
                .WithExpressionBody($"new {DbSetRefs.OrderedDbSetQueryType}(rt => query(rt).ThenBy(keySelector))"))
            .AddMethod("ThenByDescending", m => m
                .AddTypeParameter("TKey")
                .AddParameter("keySelector", ExpressionsRefs.Expression("TKey"))
                .WithReturnType(DbSetRefs.OrderedDbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Performs a subsequent ordering in descending order.")
                    .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                    .WithReturns("A new ordered query instance."))
                .WithExpressionBody($"new {DbSetRefs.OrderedDbSetQueryType}(rt => query(rt).ThenByDescending(keySelector))")))
        .AddRegion("Pagination Operations", type => type
            .AddMethod("Skip", m => m
                .AddParameter("count", "int")
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Bypasses a specified number of elements.")
                    .AddParam("count", "The number of elements to skip.")
                    .WithReturns("A new query instance with the skip applied."))
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryType}(rt => query(rt).Skip(count))"))
            .AddMethod("Take", m => m
                .AddParameter("count", "int")
                .WithReturnType(DbSetRefs.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns a specified number of elements.")
                    .AddParam("count", "The number of elements to return.")
                    .WithReturns("A new query instance with the take applied."))
                .WithExpressionBody($"new {DbSetRefs.DbSetQueryType}(rt => query(rt).Take(count))")))
        .AddRegion("Terminal Effects", type => type
            .AddCommonTerminalMethods());
}