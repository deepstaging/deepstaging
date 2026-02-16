// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Generators.Writers.LocalRefs;

/// <summary>
/// Factory methods for LanguageExt core types.
/// </summary>
internal static class LanguageExtRefs
{
    /// <summary>LanguageExt</summary>
    public static readonly NamespaceRef Namespace = NamespaceRef.From("LanguageExt");
    
    /// <summary>LanguageExt.Effects</summary>
    public static readonly NamespaceRef EffectsNamespace = NamespaceRef.From($"{Namespace}.Effects");
    
    /// <summary>static LanguageExt.Prelude</summary>
    public static readonly string PreludeNamespace = NamespaceRef.From($"{Namespace}.Prelude").AsStatic();

    /// <summary>Creates an <c>Option&lt;T&gt;</c> type reference.</summary>
    public static TypeRef Option(TypeRef innerType) => Namespace.GlobalType("Option").Of(innerType);

    /// <summary>Creates an <c>Eff&lt;RT, T&gt;</c> type reference.</summary>
    public static TypeRef Eff(TypeRef rt, TypeRef result) => Namespace.GlobalType("Eff").Of(rt, result);
    
    /// <summary><c>Eff&lt;RT, {result}&gt;</c></summary>
    public static TypeRef EffOf(TypeRef result) => Eff("RT", result);

    /// <summary><c>Eff&lt;RT, List&lt;T&gt;&gt;</c></summary>
    public static readonly TypeRef EffListT = EffOf(CollectionRefs.List("T"));

    /// <summary><c>Eff&lt;RT, T[]&gt;</c></summary>
    public static readonly TypeRef EffArrayT = EffOf("T[]");

    /// <summary><c>Eff&lt;RT, {result}[]&gt;</c></summary>
    public static TypeRef EffArray(TypeRef result) => Eff("RT", result: result.Array());

    /// <summary><c>Eff&lt;RT, Option&lt;T&gt;&gt;</c></summary>
    public static readonly TypeRef EffOptionT = EffOf(LanguageExtRefs.Option("T"));

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