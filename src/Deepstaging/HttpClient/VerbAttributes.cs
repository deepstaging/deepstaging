// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.HttpClient;

/// <summary>
/// Marks a partial method as an HTTP GET request.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class GetAttribute : Attribute
{
    /// <summary>
    /// The request path, optionally with path parameters like "/users/{id}".
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Creates a new GET attribute with the specified path.
    /// </summary>
    /// <param name="path">The request path.</param>
    public GetAttribute(string path)
    {
        Path = path;
    }
}

/// <summary>
/// Marks a partial method as an HTTP POST request.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class PostAttribute : Attribute
{
    /// <summary>
    /// The request path, optionally with path parameters like "/users/{id}".
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Creates a new POST attribute with the specified path.
    /// </summary>
    /// <param name="path">The request path.</param>
    public PostAttribute(string path)
    {
        Path = path;
    }
}

/// <summary>
/// Marks a partial method as an HTTP PUT request.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class PutAttribute : Attribute
{
    /// <summary>
    /// The request path, optionally with path parameters like "/users/{id}".
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Creates a new PUT attribute with the specified path.
    /// </summary>
    /// <param name="path">The request path.</param>
    public PutAttribute(string path)
    {
        Path = path;
    }
}

/// <summary>
/// Marks a partial method as an HTTP PATCH request.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class PatchAttribute : Attribute
{
    /// <summary>
    /// The request path, optionally with path parameters like "/users/{id}".
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Creates a new PATCH attribute with the specified path.
    /// </summary>
    /// <param name="path">The request path.</param>
    public PatchAttribute(string path)
    {
        Path = path;
    }
}

/// <summary>
/// Marks a partial method as an HTTP DELETE request.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class DeleteAttribute : Attribute
{
    /// <summary>
    /// The request path, optionally with path parameters like "/users/{id}".
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Creates a new DELETE attribute with the specified path.
    /// </summary>
    /// <param name="path">The request path.</param>
    public DeleteAttribute(string path)
    {
        Path = path;
    }
}