// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Deepstaging.HttpClient.Projection.Models;

/// <summary>
/// Model representing an HTTP client class.
/// </summary>
public sealed record HttpClientModel
{
    /// <summary>
    /// The namespace containing the client class.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The client class name.
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// The accessibility of the client class.
    /// </summary>
    public required string Accessibility { get; init; }

    /// <summary>
    /// The fully qualified name of the configuration type, if specified.
    /// </summary>
    public required string? ConfigurationType { get; init; }

    /// <summary>
    /// Optional base address for all requests.
    /// </summary>
    public required string? BaseAddress { get; init; }

    /// <summary>
    /// The HTTP requests defined on this client.
    /// </summary>
    public required ImmutableArray<HttpRequestModel> Requests { get; init; }

    /// <summary>
    /// The interface name for this client.
    /// </summary>
    public string InterfaceName => $"I{TypeName}";

    /// <summary>
    /// Whether this client has a configuration type.
    /// </summary>
    public bool HasConfiguration => ConfigurationType is not null;
}
