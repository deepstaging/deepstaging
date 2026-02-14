// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Effects.Generators.Writers;

/// <summary>
/// Shared <see cref="TypeRef"/> factories for types that appear across effect generators.
/// BCL types that have first-class support live on <see cref="TypeRef"/> itself (e.g. <see cref="TypeRef.List"/>);
/// this class covers LanguageExt types, LINQ shorthands, and composite effect types.
/// </summary>
internal static class Types
{
    // ── LanguageExt ─────────────────────────────────────────────────────

    /// <summary>Creates an <c>Option&lt;T&gt;</c> type reference.</summary>
    public static TypeRef Option(TypeRef innerType) => TypeRef.From("Option").Of(innerType);

    /// <summary>Creates an <c>Eff&lt;RT, T&gt;</c> type reference.</summary>
    public static TypeRef Eff(TypeRef rt, TypeRef result) => TypeRef.From("Eff").Of(rt, result);

    // ── LINQ Expressions ────────────────────────────────────────────────

    /// <summary>Creates an <c>Expression&lt;Func&lt;TSource, TResult&gt;&gt;</c> type reference.</summary>
    public static TypeRef Expression(TypeRef source, TypeRef result) =>
        TypeRef.Expression(TypeRef.Func(source, result));

    /// <summary><c>Expression&lt;Func&lt;T, TResult&gt;&gt;</c> — binds <c>T</c> as the source type.</summary>
    public static TypeRef Expression(TypeRef result) => Expression("T", result);

    // ── Effect composites (RT, T convention) ────────────────────────────

    /// <summary><c>Eff&lt;RT, {result}&gt;</c></summary>
    public static TypeRef EffOf(TypeRef result) => Eff("RT", result);

    /// <summary><c>Expression&lt;Func&lt;T, bool&gt;&gt;</c> — a filter predicate over the entity type.</summary>
    public static readonly TypeRef Predicate = Expression("bool");

    /// <summary><c>Eff&lt;RT, List&lt;T&gt;&gt;</c></summary>
    public static readonly TypeRef EffListT = EffOf(TypeRef.List("T"));

    /// <summary><c>Eff&lt;RT, T[]&gt;</c></summary>
    public static readonly TypeRef EffArrayT = EffOf("T[]");

    /// <summary><c>Eff&lt;RT, Option&lt;T&gt;&gt;</c></summary>
    public static readonly TypeRef EffOptionT = EffOf(Option("T"));

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

    // ── DbSetQuery types ────────────────────────────────────────────────

    /// <summary><c>Func&lt;RT, IQueryable&lt;T&gt;&gt;</c></summary>
    public static readonly TypeRef QueryFactory = TypeRef.Func("RT", TypeRef.IQueryable("T"));

    /// <summary><c>Func&lt;RT, IOrderedQueryable&lt;T&gt;&gt;</c></summary>
    public static readonly TypeRef OrderedQueryFactory = TypeRef.Func("RT", TypeRef.IOrderedQueryable("T"));

    /// <summary><c>DbSetQuery&lt;RT, {entityType}&gt;</c></summary>
    public static TypeRef QueryOf(string entityType) => TypeRef.From("DbSetQuery").Of("RT", entityType);

    /// <summary><c>DbSetQuery&lt;RT, T&gt;</c></summary>
    public static readonly TypeRef QueryType = QueryOf("T");

    /// <summary><c>DbSetOrderedQuery</c> (nested type, inherits RT and T from parent).</summary>
    public static readonly TypeRef OrderedQueryType = TypeRef.From("DbSetOrderedQuery");
}
