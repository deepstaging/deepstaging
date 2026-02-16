// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

#if NET10_0_OR_GREATER

using System.ComponentModel;
using LanguageExt;
using TUnit.Assertions.Attributes;
using TUnit.Assertions.Core;

namespace Deepstaging.Assertions;

/// <summary>
/// TUnit assertions for <see cref="Either{L,R}"/>.
/// </summary>
public static partial class EitherAssertions
{
    /// <summary>
    /// Asserts that the <see cref="Either{L,R}"/> is in the Right state.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Right")]
    public static bool IsRight<L, R>(this Either<L, R> either)
    {
        return either.IsRight;
    }

    /// <summary>
    /// Asserts that the <see cref="Either{L,R}"/> is in the Left state.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Left")]
    public static bool IsLeft<L, R>(this Either<L, R> either)
    {
        return either.IsLeft;
    }

    /// <summary>
    /// Asserts that the <see cref="Either{L,R}"/> is Right and its value equals <paramref name="expected"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Right equal to '{expected}'")]
    public static AssertionResult IsRightEqualTo<L, R>(this Either<L, R> either, R expected)
    {
        return either.Match(
            Left: l => AssertionResult.FailIf(true, $"Either was Left({l})"),
            Right: value => AssertionResult.FailIf(
                !EqualityComparer<R>.Default.Equals(value, expected),
                $"Either was Right({value})"));
    }

    /// <summary>
    /// Asserts that the <see cref="Either{L,R}"/> is Left and its value equals <paramref name="expected"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Left equal to '{expected}'")]
    public static AssertionResult IsLeftEqualTo<L, R>(this Either<L, R> either, L expected)
    {
        return either.Match(
            Left: value => AssertionResult.FailIf(
                !EqualityComparer<L>.Default.Equals(value, expected),
                $"Either was Left({value})"),
            Right: r => AssertionResult.FailIf(true, $"Either was Right({r})"));
    }

    /// <summary>
    /// Asserts that the <see cref="Either{L,R}"/> is Right and its value satisfies <paramref name="predicate"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Right matching predicate")]
    public static AssertionResult IsRightMatching<L, R>(this Either<L, R> either, Func<R, bool> predicate)
    {
        return either.Match(
            Left: l => AssertionResult.FailIf(true, $"Either was Left({l})"),
            Right: value => AssertionResult.FailIf(
                !predicate(value),
                $"Either was Right({value}) but predicate returned false"));
    }

    /// <summary>
    /// Asserts that the <see cref="Either{L,R}"/> is Left and its value satisfies <paramref name="predicate"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Left matching predicate")]
    public static AssertionResult IsLeftMatching<L, R>(this Either<L, R> either, Func<L, bool> predicate)
    {
        return either.Match(
            Left: value => AssertionResult.FailIf(
                !predicate(value),
                $"Either was Left({value}) but predicate returned false"),
            Right: r => AssertionResult.FailIf(true, $"Either was Right({r})"));
    }

    /// <summary>
    /// Asserts that the <see cref="Either{L,R}"/> is Right and its value passes the async assertions in <paramref name="assertions"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Right matching assertions")]
    public static async Task<AssertionResult> IsRightMatching<L, R>(this Either<L, R> either, Func<R, Task> assertions)
    {
        if (either.IsLeft)
            return AssertionResult.FailIf(true, $"Either was Left({either})");

        await assertions((R)either);
        return AssertionResult.Passed;
    }

    /// <summary>
    /// Asserts that the <see cref="Either{L,R}"/> is Left and its value passes the async assertions in <paramref name="assertions"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Left matching assertions")]
    public static async Task<AssertionResult> IsLeftMatching<L, R>(this Either<L, R> either, Func<L, Task> assertions)
    {
        if (either.IsRight)
            return AssertionResult.FailIf(true, $"Either was Right({either})");

        await assertions((L)either);
        return AssertionResult.Passed;
    }
}

#endif
