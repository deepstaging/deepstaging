// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

#if NET10_0_OR_GREATER

using System.ComponentModel;
using LanguageExt;
using TUnit.Assertions.Attributes;
using TUnit.Assertions.Core;

namespace Deepstaging.Assertions;

/// <summary>
/// TUnit assertions for <see cref="Fin{A}"/>.
/// </summary>
public static partial class FinAssertions
{
    /// <summary>
    /// Asserts that the <see cref="Fin{A}"/> is in a successful state.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Succ")]
    public static bool IsSucc<A>(this Fin<A> fin)
    {
        return fin.IsSucc;
    }

    /// <summary>
    /// Asserts that the <see cref="Fin{A}"/> is in a failed state.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Fail")]
    public static bool IsFail<A>(this Fin<A> fin)
    {
        return fin.IsFail;
    }

    /// <summary>
    /// Asserts that the <see cref="Fin{A}"/> is Succ and its value equals <paramref name="expected"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Succ equal to '{expected}'")]
    public static AssertionResult IsSuccEqualTo<A>(this Fin<A> fin, A expected)
    {
        return fin.Match(
            Succ: value => AssertionResult.FailIf(
                !EqualityComparer<A>.Default.Equals(value, expected),
                $"Fin was Succ({value})"),
            Fail: error => AssertionResult.FailIf(true, $"Fin was Fail({error})"));
    }

    /// <summary>
    /// Asserts that the <see cref="Fin{A}"/> is Succ and its value satisfies <paramref name="predicate"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Succ matching predicate")]
    public static AssertionResult IsSuccMatching<A>(this Fin<A> fin, Func<A, bool> predicate)
    {
        return fin.Match(
            Succ: value => AssertionResult.FailIf(
                !predicate(value),
                $"Fin was Succ({value}) but predicate returned false"),
            Fail: error => AssertionResult.FailIf(true, $"Fin was Fail({error})"));
    }

    /// <summary>
    /// Asserts that the <see cref="Fin{A}"/> is Succ and its value passes the async assertions in <paramref name="assertions"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Succ matching assertions")]
    public static async Task<AssertionResult> IsSuccMatching<A>(this Fin<A> fin, Func<A, Task> assertions)
    {
        if (fin.IsFail)
            return AssertionResult.FailIf(true, $"Fin was Fail({fin})");

        await assertions(fin.ThrowIfFail());
        return AssertionResult.Passed;
    }
}

#endif
