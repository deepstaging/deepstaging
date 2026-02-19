// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Dispatch;

/// <summary>
/// Marks a static class as containing query handler methods.
/// </summary>
/// <remarks>
/// Handler methods must be <c>static</c> and return <c>Eff&lt;TRuntime, T&gt;</c>.
/// Each method's first parameter type determines which query it handles.
/// The generator creates a strongly-typed dispatch overload for each query.
/// </remarks>
/// <example>
/// <code>
/// [QueryHandler]
/// public static class OrderQueries
/// {
///     public static Eff&lt;AppRuntime, QueryResult&lt;OrderDto&gt;&gt; Handle(GetOrders query) =&gt; ...
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class QueryHandlerAttribute : Attribute;
