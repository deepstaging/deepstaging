// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Dispatch;

/// <summary>
/// Marks a static class as containing command handler methods.
/// </summary>
/// <remarks>
/// Handler methods must be <c>static</c> and return <c>Eff&lt;TRuntime, T&gt;</c>.
/// Each method's first parameter type determines which command it handles.
/// The generator creates a strongly-typed dispatch overload for each command.
/// </remarks>
/// <example>
/// <code>
/// [CommandHandler]
/// public static class OrderCommands
/// {
///     public static Eff&lt;AppRuntime, OrderCreated&gt; Handle(CreateOrder cmd) =&gt; ...
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CommandHandlerAttribute : Attribute;
