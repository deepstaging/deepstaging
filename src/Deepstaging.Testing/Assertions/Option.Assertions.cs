// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

#if NET10_0_OR_GREATER

using System.ComponentModel;
using LanguageExt;
using TUnit.Assertions.Attributes;
using TUnit.Assertions.Core;

namespace Deepstaging.Assertions;

/// <summary>
/// TUnit assertions for <see cref="Option{A}"/>.
/// </summary>
public static partial class OptionAssertions
{
    /// <summary>
    /// Asserts that the <see cref="Option{A}"/> contains a value.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Some")]
    public static bool IsSome<A>(this Option<A> option)
    {
        return option.IsSome;
    }

    /// <summary>
    /// Asserts that the <see cref="Option{A}"/> is empty.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be None")]
    public static bool IsNone<A>(this Option<A> option)
    {
        return option.IsNone;
    }

    /// <summary>
    /// Asserts that the <see cref="Option{A}"/> is Some and its value equals <paramref name="expected"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Some equal to '{expected}'")]
    public static AssertionResult IsSomeEqualTo<A>(this Option<A> option, A expected)
    {
        if (option.IsNone)
            return AssertionResult.FailIf(true, "Option was None");

        var value = (A)option;
        return AssertionResult.FailIf(
            !EqualityComparer<A>.Default.Equals(value, expected),
            $"Option was Some({value})");
    }

    /// <summary>
    /// Asserts that the <see cref="Option{A}"/> is Some and its value satisfies <paramref name="predicate"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Some matching predicate")]
    public static AssertionResult IsSomeMatching<A>(this Option<A> option, Func<A, bool> predicate)
    {
        if (option.IsNone)
            return AssertionResult.FailIf(true, "Option was None");

        var value = (A)option;
        return AssertionResult.FailIf(
            !predicate(value),
            $"Option was Some({value}) but predicate returned false");
    }

    /// <summary>
    /// Asserts that the <see cref="Option{A}"/> is Some and its value passes the async assertions in <paramref name="assertions"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Some matching assertions")]
    public static async Task<AssertionResult> IsSomeMatching<A>(this Option<A> option, Func<A, Task> assertions)
    {
        if (option.IsNone)
            return AssertionResult.FailIf(true, "Option was None");

        await assertions((A)option);
        return AssertionResult.Passed;
    }
}

#endif
