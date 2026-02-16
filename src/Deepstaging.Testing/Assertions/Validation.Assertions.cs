// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

#if NET10_0_OR_GREATER

using System.ComponentModel;
using LanguageExt;
using TUnit.Assertions.Attributes;
using TUnit.Assertions.Core;

namespace Deepstaging.Assertions;

/// <summary>
/// TUnit assertions for <see cref="Validation{F,S}"/>.
/// </summary>
public static partial class ValidationAssertions
{
    /// <summary>
    /// Asserts that the <see cref="Validation{F,S}"/> is in a successful state.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Success")]
    public static bool IsSuccess<F, S>(this Validation<F, S> validation)
    {
        return validation.IsSuccess;
    }

    /// <summary>
    /// Asserts that the <see cref="Validation{F,S}"/> is in a failed state.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Fail")]
    public static bool IsFail<F, S>(this Validation<F, S> validation)
    {
        return validation.IsFail;
    }

    /// <summary>
    /// Asserts that the <see cref="Validation{F,S}"/> is Success and its value equals <paramref name="expected"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Success equal to '{expected}'")]
    public static AssertionResult IsSuccessEqualTo<F, S>(this Validation<F, S> validation, S expected)
    {
        return validation.Match(
            Fail: f => AssertionResult.FailIf(true, $"Validation was Fail({f})"),
            Succ: value => AssertionResult.FailIf(
                !EqualityComparer<S>.Default.Equals(value, expected),
                $"Validation was Success({value})"));
    }

    /// <summary>
    /// Asserts that the <see cref="Validation{F,S}"/> is Success and its value satisfies <paramref name="predicate"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Success matching predicate")]
    public static AssertionResult IsSuccessMatching<F, S>(this Validation<F, S> validation, Func<S, bool> predicate)
    {
        return validation.Match(
            Fail: f => AssertionResult.FailIf(true, $"Validation was Fail({f})"),
            Succ: value => AssertionResult.FailIf(
                !predicate(value),
                $"Validation was Success({value}) but predicate returned false"));
    }

    /// <summary>
    /// Asserts that the <see cref="Validation{F,S}"/> is Success and its value passes the async assertions in <paramref name="assertions"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be Success matching assertions")]
    public static async Task<AssertionResult> IsSuccessMatching<F, S>(this Validation<F, S> validation, Func<S, Task> assertions)
    {
        if (validation.IsFail)
            return AssertionResult.FailIf(true, $"Validation was Fail({validation})");

        await assertions((S)validation);
        return AssertionResult.Passed;
    }
}

#endif
