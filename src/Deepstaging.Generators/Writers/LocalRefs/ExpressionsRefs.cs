// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Generators.Writers.LocalRefs;

/// <summary>
/// Factory methods for <c>Expression&lt;Func&lt;…&gt;&gt;</c> shortcuts used in effect signatures.
/// </summary>
internal static class ExpressionsRefs
{
    /// <summary>Creates an <c>Expression&lt;Func&lt;TSource, TResult&gt;&gt;</c> type reference.</summary>
    public static TypeRef Expression(TypeRef source, TypeRef result) => LinqRefs.Expression(DelegateRefs.Func(source, result));

    /// <summary><c>Expression&lt;Func&lt;T, TResult&gt;&gt;</c> — binds <c>T</c> as the source type.</summary>
    public static TypeRef Expression(TypeRef result) => Expression("T", result);

    /// <summary><c>Expression&lt;Func&lt;T, bool&gt;&gt;</c> — a filter predicate over the entity type.</summary>
    public static readonly TypeRef Predicate = Expression("bool");
}