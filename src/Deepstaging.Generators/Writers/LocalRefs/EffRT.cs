// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Generators.Writers.LocalRefs;

/// <summary>
/// Deepstaging-specific <c>Eff&lt;RT, A&gt;</c> convenience shortcuts that assume an <c>RT</c>
/// runtime type parameter. Foundational LanguageExt refs are provided by
/// <see cref="LanguageExtRefs"/> from the <c>Deepstaging.Roslyn.LanguageExt</c> satellite package.
/// </summary>
internal static class EffRT
{
    /// <summary><c>Eff&lt;RT, {result}&gt;</c></summary>
    public static TypeRef Of(TypeRef result) => LanguageExtRefs.Eff("RT", result);

    /// <summary><c>Eff&lt;RT, List&lt;T&gt;&gt;</c></summary>
    public static readonly TypeRef ListT = Of(CollectionRefs.List("T"));

    /// <summary><c>Eff&lt;RT, T[]&gt;</c></summary>
    public static readonly TypeRef ArrayT = Of("T[]");

    /// <summary><c>Eff&lt;RT, {result}[]&gt;</c></summary>
    public static TypeRef Array(TypeRef result) => LanguageExtRefs.Eff("RT", result: result.Array());

    /// <summary><c>Eff&lt;RT, Option&lt;T&gt;&gt;</c></summary>
    public static readonly TypeRef OptionT = Of(LanguageExtRefs.Option("T"));

    /// <summary><c>Eff&lt;RT, int&gt;</c></summary>
    public static readonly TypeRef Int = Of("int");

    /// <summary><c>Eff&lt;RT, long&gt;</c></summary>
    public static readonly TypeRef Long = Of("long");

    /// <summary><c>Eff&lt;RT, bool&gt;</c></summary>
    public static readonly TypeRef Bool = Of("bool");

    /// <summary><c>Eff&lt;RT, Unit&gt;</c></summary>
    public static readonly TypeRef Unit = Of("Unit");

    /// <summary><c>Eff&lt;RT, T&gt;</c></summary>
    public static readonly TypeRef T = Of("T");

    /// <summary><c>Eff&lt;RT, TResult&gt;</c></summary>
    public static readonly TypeRef TResult = Of("TResult");
}