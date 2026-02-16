// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers;

/// <summary>
/// Shared <see cref="Roslyn.Emit.TypeRef"/> factories for types that appear across effect generators.
/// BCL types that have first-class support live on <see cref="Roslyn.Emit.TypeRef"/> itself (e.g. <see cref="Roslyn.Emit.TypeRef.Collections"/>);
/// this class covers LanguageExt types, LINQ shorthands, and composite effect types.
/// </summary>
/// <remarks>
/// Domain-specific factories are organized into nested static classes:
/// <list type="bullet">
/// <item><see cref="LanguageExt"/> — LanguageExt core types (<c>Option</c>, <c>Eff</c>)</item>
/// <item><see cref="Expressions"/> — LINQ expression shortcuts (<c>Expression&lt;Func&lt;…&gt;&gt;</c>)</item>
/// <item><see cref="Effects"/> — Composite <c>Eff&lt;RT, T&gt;</c> type references</item>
/// <item><see cref="DbSetQuery"/> — <c>DbSetQuery</c> and <c>DbSetOrderedQuery</c> types</item>
/// </list>
/// </remarks>
internal static class Types
{
    /// <summary>
    /// Factory methods for LanguageExt core types.
    /// </summary>
    public static class LanguageExt
    {
        /// <summary>Creates an <c>Option&lt;T&gt;</c> type reference.</summary>
        public static TypeRef Option(TypeRef innerType) => From("Option").Of(innerType);

        /// <summary>Creates an <c>Eff&lt;RT, T&gt;</c> type reference.</summary>
        public static TypeRef Eff(TypeRef rt, TypeRef result) => From("Eff").Of(rt, result);
    }

    /// <summary>
    /// Factory methods for <c>Expression&lt;Func&lt;…&gt;&gt;</c> shortcuts used in effect signatures.
    /// </summary>
    public static class Expressions
    {
        /// <summary>Creates an <c>Expression&lt;Func&lt;TSource, TResult&gt;&gt;</c> type reference.</summary>
        public static TypeRef Expression(TypeRef source, TypeRef result) => Linq.Expression(Delegates.Func(source, result));

        /// <summary><c>Expression&lt;Func&lt;T, TResult&gt;&gt;</c> — binds <c>T</c> as the source type.</summary>
        public static TypeRef Expression(TypeRef result) => Expression("T", result);

        /// <summary><c>Expression&lt;Func&lt;T, bool&gt;&gt;</c> — a filter predicate over the entity type.</summary>
        public static readonly TypeRef Predicate = Expression("bool");
    }

    /// <summary>
    /// Pre-built <c>Eff&lt;RT, T&gt;</c> type references for common result types.
    /// </summary>
    public static class Effects
    {
        /// <summary><c>Eff&lt;RT, {result}&gt;</c></summary>
        public static TypeRef EffOf(TypeRef result) => LanguageExt.Eff("RT", result);

        /// <summary><c>Eff&lt;RT, List&lt;T&gt;&gt;</c></summary>
        public static readonly TypeRef EffListT = EffOf(Collections.List("T"));

        /// <summary><c>Eff&lt;RT, T[]&gt;</c></summary>
        public static readonly TypeRef EffArrayT = EffOf("T[]");

        /// <summary><c>Eff&lt;RT, {result}[]&gt;</c></summary>
        public static TypeRef EffArray(TypeRef result) => LanguageExt.Eff("RT", result: result.Array());

        /// <summary><c>Eff&lt;RT, Option&lt;T&gt;&gt;</c></summary>
        public static readonly TypeRef EffOptionT = EffOf(LanguageExt.Option("T"));

        /// <summary><c>Eff&lt;RT, int&gt;</c></summary>
        public static readonly TypeRef EffInt = EffOf("int");

        /// <summary><c>Eff&lt;RT, long&gt;</c></summary>
        public static readonly TypeRef EffLong = EffOf("long");

        /// <summary><c>Eff&lt;RT, bool&gt;</c></summary>
        public static readonly TypeRef EffBool = EffOf("bool");

        /// <summary><c>Eff&lt;RT, Unit&gt;</c></summary>
        public static readonly TypeRef EffUnit = EffOf("Unit");

        /// <summary><c>Eff&lt;RT, T&gt;</c></summary>
        public static readonly TypeRef EffT = EffOf("T");

        /// <summary><c>Eff&lt;RT, TResult&gt;</c></summary>
        public static readonly TypeRef EffTResult = EffOf("TResult");
    }

    /// <summary>
    /// Factory methods for <c>DbSetQuery</c> and <c>DbSetOrderedQuery</c> types.
    /// </summary>
    public static class DbSetQuery
    {
        /// <summary><c>Func&lt;RT, IQueryable&lt;T&gt;&gt;</c></summary>
        public static readonly TypeRef DbSetQueryFactory = Delegates.Func("RT", Linq.IQueryable("T"));

        /// <summary><c>Func&lt;RT, IOrderedQueryable&lt;T&gt;&gt;</c></summary>
        public static readonly TypeRef OrderedDbSetQueryFactory = Delegates.Func("RT", Linq.IOrderedQueryable("T"));

        /// <summary><c>DbSetQuery&lt;RT, {entityType}&gt;</c></summary>
        public static TypeRef DbSetQueryOf(string entityType) => From("DbSetQuery").Of("RT", entityType);

        /// <summary><c>DbSetQuery&lt;RT, T&gt;</c></summary>
        public static readonly TypeRef DbSetQueryType = DbSetQueryOf("T");

        /// <summary><c>DbSetOrderedQuery</c> (nested type, inherits RT and T from parent).</summary>
        public static readonly TypeRef OrderedDbSetQueryType = From("DbSetOrderedQuery");
    }
}