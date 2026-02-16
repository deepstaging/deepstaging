// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging;

/// <summary>
/// Marks a partial class as a test runtime that mirrors the specified production runtime.
/// The generator discovers all capability interfaces from the target runtime's [Uses]
/// declarations and generates a test-friendly implementation with configurable service properties.
/// </summary>
/// <typeparam name="TRuntime">
/// The production runtime type decorated with [Runtime].
/// </typeparam>
/// <example>
/// <code>
/// [TestRuntime&lt;AppRuntime&gt;]
/// public partial class TestAppRuntime;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TestRuntimeAttribute<TRuntime> : Attribute;
