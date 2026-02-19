// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Dispatch;

/// <summary>
/// Marks a static partial class as a dispatch module that generates typed dispatch methods
/// for command and query handlers.
/// </summary>
/// <remarks>
/// <para>
/// The generator discovers all <see cref="CommandHandlerAttribute"/> and <see cref="QueryHandlerAttribute"/>
/// classes in the assembly and generates strongly-typed dispatch method overloads as direct
/// LanguageExt <c>Eff</c> compositions — one per command/query type. No runtime routing or lookup occurs.
/// </para>
/// <para>
/// The generated module implements <c>IEffectModule</c> and can be composed into a Runtime
/// via <c>[Uses(typeof(MyDispatch))]</c>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [DispatchModule]
/// public static partial class Dispatch;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class DispatchModuleAttribute : Attribute
{
    /// <summary>
    /// Gets or sets whether <see cref="IAutoCommittable"/> capabilities are automatically committed
    /// after command dispatch. Default is <c>true</c>.
    /// </summary>
    public bool AutoCommit { get; init; } = true;
}
