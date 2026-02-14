// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Effects.Generators.Writers;

public static class DbSetQueryWriter
{
    extension(ImmutableArray<EffectsModuleModel> modules)
    {
        public OptionalEmit WriteDbSetQueryHelper()
        {
            var queryFactory = TypeRef.Func("RT", TypeRef.From("IQueryable").Of("T"));
            var queryType = TypeRef.From("DbSetQuery").Of("RT", "T");
            var orderedQueryType = TypeRef.From("DbSetOrderedQuery").Of("RT", "T");

            return TypeBuilder
                .Parse($"public sealed class {queryType} where T : class")
                .InNamespace("Deepstaging.Effects")
                .AddUsings("System.Linq.Expressions", "LanguageExt", "static LanguageExt.Prelude", "Microsoft.EntityFrameworkCore")
                .WithXmlDoc(xml => xml
                    .WithSummary("A composable query builder for DbSet that accumulates LINQ expressions and materializes to an Eff only on terminal operations.")
                    .AddTypeParam("RT", "The runtime type that provides the DbContext capability.")
                    .AddTypeParam("T", "The entity type of the DbSet being queried."))
                .WithPrimaryConstructor(c => c.AddParameter("query", queryFactory))
                .AddFilteringMethods(queryType)
                .AddOrderingMethods(orderedQueryType)
                .AddEagerLoadingMethods(queryType)
                .AddPaginationMethods(queryType)
                .AddProjectionMethods(queryType)
                .AddSetOperationMethods(queryType)
                .AddTrackingMethods(queryType)
                .AddAggregateMethods(queryType)
                .AddTerminalEffectMethods(queryType)
                .Emit();
        }
    }

    private static TypeBuilder AddFilteringMethods(this TypeBuilder builder, TypeRef queryType) =>
        builder;

    private static TypeBuilder AddOrderingMethods(this TypeBuilder builder, TypeRef orderedQueryType) =>
        builder;

    private static TypeBuilder AddEagerLoadingMethods(this TypeBuilder builder, TypeRef queryType) =>
        builder;

    private static TypeBuilder AddPaginationMethods(this TypeBuilder builder, TypeRef queryType) =>
        builder;

    private static TypeBuilder AddProjectionMethods(this TypeBuilder builder, TypeRef queryType) =>
        builder;

    private static TypeBuilder AddSetOperationMethods(this TypeBuilder builder, TypeRef queryType) =>
        builder;

    private static TypeBuilder AddTrackingMethods(this TypeBuilder builder, TypeRef queryType) =>
        builder;

    private static TypeBuilder AddAggregateMethods(this TypeBuilder builder, TypeRef queryType) =>
        builder;

    private static TypeBuilder AddTerminalEffectMethods(this TypeBuilder builder, TypeRef queryType) =>
        builder;
}
