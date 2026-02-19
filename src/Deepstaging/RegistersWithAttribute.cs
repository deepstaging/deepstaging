// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging;

/// <summary>
/// Declares a DI registration method that the Runtime generator aggregates
/// into a unified <c>Add{Runtime}()</c> entry point.
/// </summary>
/// <remarks>
/// <para>
/// The referenced method must be a static extension method whose first parameter is
/// <c>IServiceCollection</c>, <c>WebApplicationBuilder</c>, or <c>IHostApplicationBuilder</c>.
/// </para>
/// <para>
/// The generator inspects the method's full signature and mirrors any additional parameters
/// (e.g., <c>Action&lt;TOptions&gt;?</c>) as named parameters on the generated entry point.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [RegistersWith(nameof(AddEmailServices))]
/// public static class EmailServices
/// {
///     public static IServiceCollection AddEmailServices(
///         this IServiceCollection services,
///         Action&lt;EmailOptions&gt;? configure = null) =&gt; ...
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class RegistersWithAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the static extension method that registers services.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Creates a new <see cref="RegistersWithAttribute"/>.
    /// </summary>
    /// <param name="methodName">The name of the DI registration method.</param>
    public RegistersWithAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
