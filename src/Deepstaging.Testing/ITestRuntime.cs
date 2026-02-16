// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

#if NET10_0_OR_GREATER

namespace Deepstaging;

/// <summary>
/// Interface for generated test runtimes that support parameterless factory creation.
/// Implemented automatically by the <c>[TestRuntime]</c> source generator.
/// </summary>
/// <typeparam name="TSelf">The concrete test runtime type (CRTP).</typeparam>
public interface ITestRuntime<TSelf> where TSelf : ITestRuntime<TSelf>
{
    /// <summary>
    /// Creates a new test runtime instance with all capabilities unconfigured.
    /// </summary>
    static abstract TSelf Create();
}

#endif
