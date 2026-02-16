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
    public static EffTypeRef Of(TypeRef result) => LanguageExtRefs.Eff("RT", result);

    /// <summary><c>Eff&lt;RT, List&lt;T&gt;&gt;</c></summary>
    public static readonly EffTypeRef ListT = Of(CollectionRefs.List("T"));

    /// <summary><c>Eff&lt;RT, T[]&gt;</c></summary>
    public static readonly EffTypeRef ArrayT = Of("T[]");

    /// <summary><c>Eff&lt;RT, {result}[]&gt;</c></summary>
    public static EffTypeRef Array(TypeRef result) => LanguageExtRefs.Eff("RT", result: result.Array());

    /// <summary><c>Eff&lt;RT, Option&lt;T&gt;&gt;</c></summary>
    public static readonly EffTypeRef OptionT = Of(LanguageExtRefs.Option("T"));

    /// <summary><c>Eff&lt;RT, int&gt;</c></summary>
    public static readonly EffTypeRef Int = Of("int");

    /// <summary><c>Eff&lt;RT, long&gt;</c></summary>
    public static readonly EffTypeRef Long = Of("long");

    /// <summary><c>Eff&lt;RT, bool&gt;</c></summary>
    public static readonly EffTypeRef Bool = Of("bool");

    /// <summary><c>Eff&lt;RT, Unit&gt;</c></summary>
    public static readonly EffTypeRef Unit = Of("Unit");

    /// <summary><c>Eff&lt;RT, T&gt;</c></summary>
    public static readonly EffTypeRef T = Of("T");

    /// <summary><c>Eff&lt;RT, TResult&gt;</c></summary>
    public static readonly EffTypeRef TResult = Of("TResult");
}