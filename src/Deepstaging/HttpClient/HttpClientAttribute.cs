// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.HttpClient;

/// <summary>
/// Marks a partial class as an HTTP client that will have typed HTTP methods generated.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class HttpClientAttribute : Attribute
{
    /// <summary>
    /// Optional base address for all requests.
    /// </summary>
    public string? BaseAddress { get; set; }
}

/// <summary>
/// Marks a partial class as an HTTP client with a typed configuration.
/// </summary>
/// <typeparam name="TConfiguration">The configuration type to inject.</typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class HttpClientAttribute<TConfiguration> : Attribute
{
    /// <summary>
    /// Optional base address for all requests.
    /// </summary>
    public string? BaseAddress { get; set; }
}