// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Generators.Writers.LocalRefs;

/// <summary>
/// Factory methods for <c>DbSetQuery</c> and <c>DbSetOrderedQuery</c> types.
/// </summary>
internal static class DbSetRefs
{
    /// <summary><c>Func&lt;RT, IQueryable&lt;T&gt;&gt;</c></summary>
    public static readonly TypeRef DbSetQueryFactory = DelegateRefs.Func("RT", LinqRefs.IQueryable("T"));

    /// <summary><c>Func&lt;RT, IOrderedQueryable&lt;T&gt;&gt;</c></summary>
    public static readonly TypeRef OrderedDbSetQueryFactory = DelegateRefs.Func("RT", LinqRefs.IOrderedQueryable("T"));

    /// <summary><c>DbSetQuery&lt;RT, {entityType}&gt;</c></summary>
    public static TypeRef DbSetQueryOf(string entityType) => TypeRef.From("DbSetQuery").Of("RT", entityType);

    /// <summary><c>DbSetQuery&lt;RT, T&gt;</c></summary>
    public static readonly TypeRef DbSetQueryType = DbSetQueryOf("T");

    /// <summary><c>DbSetOrderedQuery</c> (nested type, inherits RT and T from parent).</summary>
    public static readonly TypeRef OrderedDbSetQueryType = TypeRef.From("DbSetOrderedQuery");
}