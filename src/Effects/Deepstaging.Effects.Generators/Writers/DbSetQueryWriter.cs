// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using static Deepstaging.Effects.Generators.Writers.Types;

namespace Deepstaging.Effects.Generators.Writers;

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
        public OptionalEmit WriteDbSetQueryHelper()
        {
            return TypeBuilder
                .Parse($"public sealed class {QueryType} where T : class")
                .InNamespace("Deepstaging.Effects")
                .AddUsings(
                    "LanguageExt",
                    "System.Linq",
                    "System.Linq.Expressions",
                    "System.Threading",
                    "static LanguageExt.Prelude",
                    "Microsoft.EntityFrameworkCore")
                .WithXmlDoc(xml => xml
                    .WithSummary("A composable query builder for DbSet that accumulates LINQ expressions and materializes to an Eff only on terminal operations.")
                    .AddTypeParam("RT", "The runtime type that provides the DbContext capability.")
                    .AddTypeParam("T", "The entity type of the DbSet being queried."))
                .WithPrimaryConstructor(c => c.AddParameter("query", QueryFactory))
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
    }

    // ========== DbSetQuery Methods ==========

    private static TypeBuilder AddFilteringMethods(this TypeBuilder builder) =>
        builder.AddRegion("Filtering Operations", type => type
            .AddMethod("Where", m => m
                .AddParameter("predicate", Predicate)
                .WithReturnType(QueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Filters the sequence based on a predicate.")
                    .AddParam("predicate", "The LINQ expression representing the filter condition.")
                    .WithReturns("A new query instance with the added filter."))
                .WithExpressionBody($"new {QueryType}(rt => query(rt).Where(predicate))")));

    private static TypeBuilder AddOrderingMethods(this TypeBuilder builder) =>
        builder.AddRegion("Ordering Operations", type => type
            .AddMethod("OrderBy", m => m
                .AddTypeParameter("TKey")
                .AddParameter("keySelector", Expression("TKey"))
                .WithReturnType(OrderedQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Sorts the elements in ascending order.")
                    .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                    .WithReturns("A new ordered query instance."))
                .WithExpressionBody($"new {OrderedQueryType}(rt => query(rt).OrderBy(keySelector))"))
            .AddMethod("OrderByDescending", m => m
                .AddTypeParameter("TKey")
                .AddParameter("keySelector", Expression("TKey"))
                .WithReturnType(OrderedQueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Sorts the elements in descending order.")
                    .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                    .WithReturns("A new ordered query instance."))
                .WithExpressionBody($"new {OrderedQueryType}(rt => query(rt).OrderByDescending(keySelector))")));

    private static TypeBuilder AddEagerLoadingMethods(this TypeBuilder builder) =>
        builder.AddRegion("Eager Loading Operations", type => type
            .AddMethod("Include", m => m
                .AddTypeParameter("TProperty")
                .AddParameter("navigationPropertyPath", Expression("TProperty"))
                .WithReturnType(QueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Includes a related entity in the query results.")
                    .AddParam("navigationPropertyPath", "The navigation property to include.")
                    .WithReturns("A new query instance with the eager loading applied."))
                .WithExpressionBody($"new {QueryType}(rt => query(rt).Include(navigationPropertyPath))"))
            .AddMethod("Include", m => m
                .AddParameter("navigationPropertyPath", "string")
                .WithReturnType(QueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Includes a related entity using a string path.")
                    .AddParam("navigationPropertyPath", "The string navigation property path to include.")
                    .WithReturns("A new query instance with the eager loading applied."))
                .WithExpressionBody($"new {QueryType}(rt => query(rt).Include(navigationPropertyPath))")));

    private static TypeBuilder AddPaginationMethods(this TypeBuilder builder) =>
        builder.AddRegion("Pagination Operations", type => type
            .AddMethod("Skip", m => m
                .AddParameter("count", "int")
                .WithReturnType(QueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Bypasses a specified number of elements.")
                    .AddParam("count", "The number of elements to skip.")
                    .WithReturns("A new query instance with the skip applied."))
                .WithExpressionBody($"new {QueryType}(rt => query(rt).Skip(count))"))
            .AddMethod("Take", m => m
                .AddParameter("count", "int")
                .WithReturnType(QueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns a specified number of elements.")
                    .AddParam("count", "The number of elements to return.")
                    .WithReturns("A new query instance with the take applied."))
                .WithExpressionBody($"new {QueryType}(rt => query(rt).Take(count))")));

    private static TypeBuilder AddProjectionMethods(this TypeBuilder builder) =>
        builder.AddRegion("Projection Operations", type => type
            .AddMethod(MethodBuilder
                .Parse(
                    $"public {QueryOf("TResult")} Select<TResult>({Expression("TResult")} selector) where TResult : class")
                .WithXmlDoc(xml => xml
                    .WithSummary("Projects each element to a new form.")
                    .AddParam("selector", "The projection expression.")
                    .WithReturns("A new query instance projecting to the result type."))
                .WithExpressionBody($"new {QueryOf("TResult")}(rt => query(rt).Select(selector))"))
            .AddMethod(MethodBuilder
                .Parse(
                    $"public {QueryOf("TResult")} SelectMany<TResult>({Expression(TypeRef.IEnumerable("TResult"))} selector) where TResult : class")
                .WithXmlDoc(xml => xml
                    .WithSummary("Projects and flattens sequences.")
                    .AddParam("selector", "The projection expression.")
                    .WithReturns("A new query instance projecting and flattening to the result type."))
                .WithExpressionBody($"new {QueryOf("TResult")}(rt => query(rt).SelectMany(selector))")));

    private static TypeBuilder AddSetOperationMethods(this TypeBuilder builder) =>
        builder.AddRegion("Set Operations", type => type
            .AddMethod("Distinct", m => m
                .WithReturnType(QueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Returns distinct elements.")
                    .WithReturns("A new query instance with distinct applied."))
                .WithExpressionBody($"new {QueryType}(rt => Queryable.Distinct(query(rt)))")));

    private static TypeBuilder AddTrackingMethods(this TypeBuilder builder) =>
        builder.AddRegion("Tracking Operations", type => type
            .AddMethod("AsNoTracking", m => m
                .WithReturnType(QueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Disables change tracking for the query.")
                    .WithReturns("A new query instance with no tracking."))
                .WithExpressionBody($"new {QueryType}(rt => query(rt).AsNoTracking())"))
            .AddMethod("AsNoTrackingWithIdentityResolution", m => m
                .WithReturnType(QueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Disables change tracking but enables identity resolution.")
                    .WithReturns("A new query instance with identity resolution tracking."))
                .WithExpressionBody($"new {QueryType}(rt => query(rt).AsNoTrackingWithIdentityResolution())"))
            .AddMethod("AsTracking", m => m
                .WithReturnType(QueryType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Enables change tracking.")
                    .WithReturns("A new query instance with tracking enabled."))
                .WithExpressionBody($"new {QueryType}(rt => query(rt).AsTracking())")));

    private static TypeBuilder AddTerminalEffectMethods(this TypeBuilder builder) =>
        builder.AddRegion("Terminal Effects", type => type
            .AddCommonTerminalMethods()
            .AddMethod(MethodBuilder
                .Parse(
                    $"public {EffOptionT} FirstOrNoneAsync({Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc("Returns the first element matching the predicate, or None if not found.")
                .WithExpressionBody(
                    $"{EffOptionT}.LiftIO(async rt => Optional(await query(rt).FirstOrDefaultAsync(predicate, token)))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffOptionT} SingleOrNoneAsync(CancellationToken token = default)")
                .WithXmlDoc("Returns the single element, or None if empty. Throws if more than one.")
                .WithExpressionBody(
                    $"{EffOptionT}.LiftIO(async rt => Optional(await query(rt).SingleOrDefaultAsync(token)))"))
            .AddMethod(MethodBuilder
                .Parse(
                    $"public {EffOptionT} SingleOrNoneAsync({Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc("Returns the single element matching the predicate, or None. Throws if more than one.")
                .WithExpressionBody(
                    $"{EffOptionT}.LiftIO(async rt => Optional(await query(rt).SingleOrDefaultAsync(predicate, token)))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffInt} CountAsync(CancellationToken token = default)")
                .WithXmlDoc("Returns the count of elements.")
                .WithExpressionBody($"{EffInt}.LiftIO(async rt => await query(rt).CountAsync(token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffInt} CountAsync({Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc("Returns the count of elements matching the predicate.")
                .WithExpressionBody($"{EffInt}.LiftIO(async rt => await query(rt).CountAsync(predicate, token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffLong} LongCountAsync(CancellationToken token = default)")
                .WithXmlDoc("Returns the long count of elements.")
                .WithExpressionBody($"{EffLong}.LiftIO(async rt => await query(rt).LongCountAsync(token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffBool} AnyAsync(CancellationToken token = default)")
                .WithXmlDoc("Returns true if any elements exist.")
                .WithExpressionBody($"{EffBool}.LiftIO(async rt => await query(rt).AnyAsync(token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffBool} AnyAsync({Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc("Returns true if any elements match the predicate.")
                .WithExpressionBody($"{EffBool}.LiftIO(async rt => await query(rt).AnyAsync(predicate, token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffBool} AllAsync({Predicate} predicate, CancellationToken token = default)")
                .WithXmlDoc("Returns true if all elements match the predicate.")
                .WithExpressionBody($"{EffBool}.LiftIO(async rt => await query(rt).AllAsync(predicate, token))")));

    private static TypeBuilder AddAggregateMethods(this TypeBuilder builder) =>
        builder.AddRegion("Aggregate Operations", type => type
            .AddMethod(MethodBuilder
                .Parse($"public {EffT} MaxAsync(CancellationToken token = default)")
                .WithXmlDoc("Returns the maximum value.")
                .WithExpressionBody($"{EffT}.LiftIO(async rt => (await query(rt).MaxAsync(token))!)"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffTResult} MaxAsync<TResult>({Expression("TResult")} selector, CancellationToken token = default)")
                .WithXmlDoc("Returns the maximum value of the selected property.")
                .WithExpressionBody($"{EffTResult}.LiftIO(async rt => (await query(rt).MaxAsync(selector, token))!)"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffT} MinAsync(CancellationToken token = default)")
                .WithXmlDoc("Returns the minimum value.")
                .WithExpressionBody($"{EffT}.LiftIO(async rt => (await query(rt).MinAsync(token))!)"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffTResult} MinAsync<TResult>({Expression("TResult")} selector, CancellationToken token = default)")
                .WithXmlDoc("Returns the minimum value of the selected property.")
                .WithExpressionBody($"{EffTResult}.LiftIO(async rt => (await query(rt).MinAsync(selector, token))!)")));

    // ========== Shared terminal methods (used by both DbSetQuery and DbSetOrderedQuery) ==========

    private static TypeBuilder AddCommonTerminalMethods(this TypeBuilder builder) =>
        builder
            .AddMethod(MethodBuilder
                .Parse($"public {EffListT} ToListAsync(CancellationToken token = default)")
                .WithXmlDoc("Executes the query and returns all results as a list.")
                .WithExpressionBody($"{EffListT}.LiftIO(async rt => await query(rt).ToListAsync(token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffArrayT} ToArrayAsync(CancellationToken token = default)")
                .WithXmlDoc("Executes the query and returns all results as an array.")
                .WithExpressionBody($"{EffArrayT}.LiftIO(async rt => await query(rt).ToArrayAsync(token))"))
            .AddMethod(MethodBuilder
                .Parse($"public {EffOptionT} FirstOrNoneAsync(CancellationToken token = default)")
                .WithXmlDoc("Returns the first element, or None if empty.")
                .WithExpressionBody(
                    $"{EffOptionT}.LiftIO(async rt => Optional(await query(rt).FirstOrDefaultAsync(token)))"));

    // ========== Nested DbSetOrderedQuery ==========

    private static TypeBuilder BuildOrderedQueryType() =>
        TypeBuilder.Parse("public sealed class DbSetOrderedQuery")
            .WithXmlDoc(xml => xml
                .WithSummary("An ordered query builder that supports ThenBy operations."))
            .WithPrimaryConstructor(c => c.AddParameter("query", OrderedQueryFactory))
            .AddRegion("Ordering Operations", type => type
                .AddMethod("ThenBy", m => m
                    .AddTypeParameter("TKey")
                    .AddParameter("keySelector", Expression("TKey"))
                    .WithReturnType(OrderedQueryType)
                    .WithXmlDoc(xml => xml
                        .WithSummary("Performs a subsequent ordering in ascending order.")
                        .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                        .WithReturns("A new ordered query instance."))
                    .WithExpressionBody($"new {OrderedQueryType}(rt => query(rt).ThenBy(keySelector))"))
                .AddMethod("ThenByDescending", m => m
                    .AddTypeParameter("TKey")
                    .AddParameter("keySelector", Expression("TKey"))
                    .WithReturnType(OrderedQueryType)
                    .WithXmlDoc(xml => xml
                        .WithSummary("Performs a subsequent ordering in descending order.")
                        .AddParam("keySelector", "The LINQ expression representing the key to sort by.")
                        .WithReturns("A new ordered query instance."))
                    .WithExpressionBody($"new {OrderedQueryType}(rt => query(rt).ThenByDescending(keySelector))")))
            .AddRegion("Pagination Operations", type => type
                .AddMethod("Skip", m => m
                    .AddParameter("count", "int")
                    .WithReturnType(QueryType)
                    .WithXmlDoc(xml => xml
                        .WithSummary("Bypasses a specified number of elements.")
                        .AddParam("count", "The number of elements to skip.")
                        .WithReturns("A new query instance with the skip applied."))
                    .WithExpressionBody($"new {QueryType}(rt => query(rt).Skip(count))"))
                .AddMethod("Take", m => m
                    .AddParameter("count", "int")
                    .WithReturnType(QueryType)
                    .WithXmlDoc(xml => xml
                        .WithSummary("Returns a specified number of elements.")
                        .AddParam("count", "The number of elements to return.")
                        .WithReturns("A new query instance with the take applied."))
                    .WithExpressionBody($"new {QueryType}(rt => query(rt).Take(count))")))
            .AddRegion("Terminal Effects", type => type
                .AddCommonTerminalMethods());
}
