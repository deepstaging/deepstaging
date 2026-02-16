// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.HttpClient;

/// <summary>
/// Marks a parameter as a path parameter. Path parameters are substituted into the URL path.
/// If not specified, parameters matching {name} in the path are automatically treated as path parameters.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class PathAttribute : Attribute;

/// <summary>
/// Marks a parameter as a query string parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class QueryAttribute : Attribute
{
    /// <summary>
    /// Optional name override for the query parameter. If not specified, uses the parameter name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Creates a new Query attribute using the parameter name.
    /// </summary>
    public QueryAttribute()
    {
    }

    /// <summary>
    /// Creates a new Query attribute with a custom query parameter name.
    /// </summary>
    /// <param name="name">The query parameter name.</param>
    public QueryAttribute(string name)
    {
        Name = name;
    }
}

/// <summary>
/// Marks a parameter as an HTTP header.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class HeaderAttribute : Attribute
{
    /// <summary>
    /// The HTTP header name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Creates a new Header attribute with the specified header name.
    /// </summary>
    /// <param name="name">The HTTP header name (e.g., "X-Request-Id").</param>
    public HeaderAttribute(string name)
    {
        Name = name;
    }
}

/// <summary>
/// Marks a parameter as the request body. Only one parameter per method can be marked as body.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class BodyAttribute : Attribute;