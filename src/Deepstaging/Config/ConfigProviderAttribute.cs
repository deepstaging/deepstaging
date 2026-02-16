// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Config;

/// <summary>
/// Marks a class as a configuration provider. The generator will create an interface and partial
/// implementation that binds to the specified configuration section.
/// </summary>
/// <remarks>
/// The <see cref="Section"/> name is optional. If omitted, it is inferred by stripping the
/// <c>ConfigProvider</c> suffix from the class name (e.g. <c>SlackConfigProvider</c> → <c>"Slack"</c>).
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ConfigProviderAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the configuration section name.
    /// When <c>null</c>, the section is inferred from the class name.
    /// </summary>
    public string? Section { get; set; }
}

/// <summary>
/// Declares a configuration type to expose from a <see cref="ConfigProviderAttribute"/> class.
/// The generator introspects <typeparamref name="T"/> for its public instance properties.
/// </summary>
/// <typeparam name="T">The configuration type to expose.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ExposesAttribute<T>() : Attribute;