// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Effects;

using static Types;

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
                .Parse($"public sealed class {DbSetQuery.DbSetQueryType} where T : class")
                .InNamespace("Deepstaging.Effects")
                .AddUsings(
                    "LanguageExt",
                    "System.Linq",
                    "System.Linq.Expressions",
                    "System.Threading",
                    "static LanguageExt.Prelude",
                    "Microsoft.EntityFrameworkCore"
                )
                .WithXmlDoc(xml => xml
                    .WithSummary("A composable query builder for DbSet that accumulates LINQ expressions and materializes to an Eff only on terminal operations.")
                    .AddTypeParam("RT", "The runtime type that provides the DbContext capability.")
                    .AddTypeParam("T", "The entity type of the DbSet being queried.")
                )
                .WithPrimaryConstructor(c => c.AddParameter("query", DbSetQuery.DbSetQueryFactory))
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
                .AddParameter("predicate", Expressions.Predicate)
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => query(rt).Where(predicate))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Filters the sequence based on a predicate.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .WithReturns("A new query instance with the added filter."))));

    private static TypeBuilder AddOrderingMethods(this TypeBuilder builder) => builder
        .AddRegion("Ordering Operations", type => type
            .AddMethod("OrderBy", m => m
                .AddTypeParameter("TKey")
                .AddParameter("keySelector", Expressions.Expression("TKey"))
                .WithReturnType(DbSetQuery.OrderedDbSetQueryType)
                .WithExpressionBody($"new {DbSetQuery.OrderedDbSetQueryType}(rt => query(rt).OrderBy(keySelector))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Sorts the elements in ascending order.")
                    .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                    .WithReturns("A new ordered query instance.")))
            .AddMethod("OrderByDescending", m => m
                .AddTypeParameter("TKey")
                .AddParameter("keySelector", Expressions.Expression("TKey"))
                .WithReturnType(DbSetQuery.OrderedDbSetQueryType)
                .WithExpressionBody($"new {DbSetQuery.OrderedDbSetQueryType}(rt => query(rt).OrderByDescending(keySelector))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Sorts the elements in descending order.")
                    .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                    .WithReturns("A new ordered query instance."))));

    private static TypeBuilder AddEagerLoadingMethods(this TypeBuilder builder) => builder
        .AddRegion("Eager Loading Operations", type => type
            .AddMethod("Include", m => m
                .AddTypeParameter("TProperty")
                .AddParameter("navigationPropertyPath", Expressions.Expression("TProperty"))
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => query(rt).Include(navigationPropertyPath))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Includes a related entity in the query results.")
                    .AddParam("navigationPropertyPath", "The navigation property to include.")
                    .WithReturns("A new query instance with the eager loading applied.")
                ))
            .AddMethod("Include", m => m
                .AddParameter("navigationPropertyPath", "string")
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => query(rt).Include(navigationPropertyPath))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Includes a related entity using a string path.")
                    .AddParam("navigationPropertyPath", "The string navigation property path to include.")
                    .WithReturns("A new query instance with the eager loading applied.")
                )));

    private static TypeBuilder AddPaginationMethods(this TypeBuilder builder) => builder
        .AddRegion("Pagination Operations", type => type
            .AddMethod("Skip", m => m
                .AddParameter("count", "int")
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => query(rt).Skip(count))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Bypasses a specified number of elements.")
                    .AddParam("count", "The number of elements to skip.")
                    .WithReturns("A new query instance with the skip applied.")
                )
            )
            .AddMethod("Take", m => m
                .AddParameter("count", "int")
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => query(rt).Take(count))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns a specified number of elements.")
                    .AddParam("count", "The number of elements to return.")
                    .WithReturns("A new query instance with the take applied.")
                )));

    // TODO: Finish reformatting Select and SelectMany — complex generic signatures
    private static TypeBuilder AddProjectionMethods(this TypeBuilder builder) => builder
        .AddRegion("Projection Operations", type => type
            .AddMethod(MethodBuilder
                .Parse($"public {DbSetQuery.DbSetQueryOf("TResult")} Select<TResult>({Expressions.Expression("TResult")} selector) where TResult : class")
                .WithXmlDoc(xml => xml
                    .WithSummary("Projects each element to a new form.")
                    .AddParam("selector", "The projection expression.")
                    .WithReturns("A new query instance projecting to the result type."))
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryOf("TResult")}(rt => query(rt).Select(selector))"))
            .AddMethod(MethodBuilder
                .Parse(
                    $"public {DbSetQuery.DbSetQueryOf("TResult")} SelectMany<TResult>({Expressions.Expression(TypeRef.Collections.IEnumerable("TResult"))} selector) where TResult : class")
                .WithXmlDoc(xml => xml
                    .WithSummary("Projects and flattens sequences.")
                    .AddParam("selector", "The projection expression.")
                    .WithReturns("A new query instance projecting and flattening to the result type."))
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryOf("TResult")}(rt => query(rt).SelectMany(selector))")));

    private static TypeBuilder AddSetOperationMethods(this TypeBuilder builder) => builder
        .AddRegion("Set Operations", type => type
            .AddMethod("Distinct", m => m
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns distinct elements.")
                    .WithReturns("A new query instance with distinct applied."))
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => Queryable.Distinct(query(rt)))")));

    private static TypeBuilder AddTrackingMethods(this TypeBuilder builder) => builder
        .AddRegion("Tracking Operations", type => type
            .AddMethod("AsNoTracking", m => m
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Disables change tracking for the query.")
                    .WithReturns("A new query instance with no tracking."))
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => query(rt).AsNoTracking())"))
            .AddMethod("AsNoTrackingWithIdentityResolution", m => m
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Disables change tracking but enables identity resolution.")
                    .WithReturns("A new query instance with identity resolution tracking."))
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => query(rt).AsNoTrackingWithIdentityResolution())"))
            .AddMethod("AsTracking", m => m
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Enables change tracking.")
                    .WithReturns("A new query instance with tracking enabled."))
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => query(rt).AsTracking())")));

    private static TypeBuilder AddTerminalEffectMethods(this TypeBuilder builder) => builder
        .AddRegion("Terminal Effects", type => type
            .AddCommonTerminalMethods()
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffOptionT} FirstOrNoneAsync({Expressions.Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the first element matching the predicate, or None if not found.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the matching element wrapped in an Option, or None."))
                .WithExpressionBody($"{Effects.EffOptionT}.LiftIO(async rt => Optional(await query(rt).FirstOrDefaultAsync(predicate, token)))"))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffOptionT} SingleOrNoneAsync(CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the single element, or None if empty. Throws if more than one element exists.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the single element wrapped in an Option, or None."))
                .WithExpressionBody($"{Effects.EffOptionT}.LiftIO(async rt => Optional(await query(rt).SingleOrDefaultAsync(token)))"))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffOptionT} SingleOrNoneAsync({Expressions.Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the single element matching the predicate, or None. Throws if more than one element matches.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the matching element wrapped in an Option, or None."))
                .WithExpressionBody($"{Effects.EffOptionT}.LiftIO(async rt => Optional(await query(rt).SingleOrDefaultAsync(predicate, token)))"))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffInt} CountAsync(CancellationToken token = default)")
                .WithExpressionBody($"{Effects.EffInt}.LiftIO(async rt => await query(rt).CountAsync(token))")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the count of elements in the sequence.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the number of elements.")))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffInt} CountAsync({Expressions.Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the count of elements matching the predicate.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the number of matching elements."))
                .WithExpressionBody($"{Effects.EffInt}.LiftIO(async rt => await query(rt).CountAsync(predicate, token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffLong} LongCountAsync(CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the count of elements as a 64-bit integer.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the number of elements as a <see langword=\"long\" />."))
                .WithExpressionBody($"{Effects.EffLong}.LiftIO(async rt => await query(rt).LongCountAsync(token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffBool} AnyAsync(CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Determines whether the sequence contains any elements.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields <see langword=\"true\" /> if the sequence contains any elements."))
                .WithExpressionBody($"{Effects.EffBool}.LiftIO(async rt => await query(rt).AnyAsync(token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffBool} AnyAsync({Expressions.Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Determines whether any element satisfies the predicate.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields <see langword=\"true\" /> if any element matches the predicate."))
                .WithExpressionBody($"{Effects.EffBool}.LiftIO(async rt => await query(rt).AnyAsync(predicate, token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffBool} AllAsync({Expressions.Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Determines whether all elements satisfy the predicate.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields <see langword=\"true\" /> if every element matches the predicate."))
                .WithExpressionBody($"{Effects.EffBool}.LiftIO(async rt => await query(rt).AllAsync(predicate, token))")));

    private static TypeBuilder AddAggregateMethods(this TypeBuilder builder) => builder
        .AddRegion("Aggregate Operations", type => type
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffT} MaxAsync(CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the maximum value in the sequence.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the maximum value."))
                .WithExpressionBody($"{Effects.EffT}.LiftIO(async rt => (await query(rt).MaxAsync(token))!)"))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffTResult} MaxAsync<TResult>({Expressions.Expression("TResult")} selector, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the maximum value of the selected projection.")
                    .AddTypeParam("TResult", "The type of the projected value.")
                    .AddParam("selector", "The projection expression to apply to each element.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the maximum projected value."))
                .WithExpressionBody($"{Effects.EffTResult}.LiftIO(async rt => (await query(rt).MaxAsync(selector, token))!)"))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffT} MinAsync(CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the minimum value in the sequence.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the minimum value."))
                .WithExpressionBody($"{Effects.EffT}.LiftIO(async rt => (await query(rt).MinAsync(token))!)"))
            .AddMethod(MethodBuilder
                .Parse($"public {Effects.EffTResult} MinAsync<TResult>({Expressions.Expression("TResult")} selector, CancellationToken token = default)")
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns the minimum value of the selected projection.")
                    .AddTypeParam("TResult", "The type of the projected value.")
                    .AddParam("selector", "The projection expression to apply to each element.")
                    .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                    .WithReturns("An effect that yields the minimum projected value."))
                .WithExpressionBody($"{Effects.EffTResult}.LiftIO(async rt => (await query(rt).MinAsync(selector, token))!)")));

    // ========== Shared terminal methods (used by both DbSetQuery and DbSetOrderedQuery) ==========

    private static TypeBuilder AddCommonTerminalMethods(this TypeBuilder builder) => builder
        .AddMethod(MethodBuilder
            .Parse($"public {Effects.EffListT} ToListAsync(CancellationToken token = default)")
            .WithXmlDoc(xml => xml
                .WithSummary("Executes the query and returns all results as a list.")
                .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                .WithReturns("An effect that yields a list of all matching elements."))
            .WithExpressionBody($"{Effects.EffListT}.LiftIO(async rt => await query(rt).ToListAsync(token))"))
        .AddMethod(MethodBuilder
            .Parse($"public {Effects.EffArrayT} ToArrayAsync(CancellationToken token = default)")
            .WithXmlDoc(xml => xml
                .WithSummary("Executes the query and returns all results as an array.")
                .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                .WithReturns("An effect that yields an array of all matching elements."))
            .WithExpressionBody($"{Effects.EffArrayT}.LiftIO(async rt => await query(rt).ToArrayAsync(token))"))
        .AddMethod(MethodBuilder
            .Parse($"public {Effects.EffOptionT} FirstOrNoneAsync(CancellationToken token = default)")
            .WithXmlDoc(xml => xml
                .WithSummary("Returns the first element, or None if the sequence is empty.")
                .AddParam("token", "A cancellation token to observe while waiting for the task to complete.")
                .WithReturns("An effect that yields the first element wrapped in an Option, or None."))
            .WithExpressionBody($"{Effects.EffOptionT}.LiftIO(async rt => Optional(await query(rt).FirstOrDefaultAsync(token)))"));

    // ========== Nested DbSetOrderedQuery ==========

    private static TypeBuilder BuildOrderedQueryType() => TypeBuilder
        .Parse("public sealed class DbSetOrderedQuery")
        .WithXmlDoc("An ordered query builder that supports ThenBy operations.")
        .WithPrimaryConstructor(c => c.AddParameter("query", DbSetQuery.OrderedDbSetQueryFactory))
        .AddRegion("Ordering Operations", type => type
            .AddMethod("ThenBy", m => m
                .AddTypeParameter("TKey")
                .AddParameter("keySelector", Expressions.Expression("TKey"))
                .WithReturnType(DbSetQuery.OrderedDbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Performs a subsequent ordering in ascending order.")
                    .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                    .WithReturns("A new ordered query instance."))
                .WithExpressionBody($"new {DbSetQuery.OrderedDbSetQueryType}(rt => query(rt).ThenBy(keySelector))"))
            .AddMethod("ThenByDescending", m => m
                .AddTypeParameter("TKey")
                .AddParameter("keySelector", Expressions.Expression("TKey"))
                .WithReturnType(DbSetQuery.OrderedDbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Performs a subsequent ordering in descending order.")
                    .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                    .WithReturns("A new ordered query instance."))
                .WithExpressionBody($"new {DbSetQuery.OrderedDbSetQueryType}(rt => query(rt).ThenByDescending(keySelector))")))
        .AddRegion("Pagination Operations", type => type
            .AddMethod("Skip", m => m
                .AddParameter("count", "int")
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Bypasses a specified number of elements.")
                    .AddParam("count", "The number of elements to skip.")
                    .WithReturns("A new query instance with the skip applied."))
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => query(rt).Skip(count))"))
            .AddMethod("Take", m => m
                .AddParameter("count", "int")
                .WithReturnType(DbSetQuery.DbSetQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns a specified number of elements.")
                    .AddParam("count", "The number of elements to return.")
                    .WithReturns("A new query instance with the take applied."))
                .WithExpressionBody($"new {DbSetQuery.DbSetQueryType}(rt => query(rt).Take(count))")))
        .AddRegion("Terminal Effects", type => type
            .AddCommonTerminalMethods());
}