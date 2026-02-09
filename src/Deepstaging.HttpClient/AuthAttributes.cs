// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.HttpClient;

/// <summary>
/// Configures Bearer token authentication for the HTTP client.
/// The token is obtained from an injected ITokenProvider.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class BearerAuthAttribute : Attribute;

/// <summary>
/// Configures API key authentication for the HTTP client.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ApiKeyAuthAttribute : Attribute
{
    /// <summary>
    /// The HTTP header name for the API key.
    /// </summary>
    public string HeaderName { get; }

    /// <summary>
    /// Optional property name on the configuration type to get the API key from.
    /// </summary>
    public string? ConfigProperty { get; set; }

    /// <summary>
    /// Creates a new ApiKeyAuth attribute with the specified header name.
    /// </summary>
    /// <param name="headerName">The header name (e.g., "X-Api-Key").</param>
    public ApiKeyAuthAttribute(string headerName) => HeaderName = headerName;
}

/// <summary>
/// Configures Basic authentication for the HTTP client.
/// Username and password are obtained from the configuration type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class BasicAuthAttribute : Attribute
{
    /// <summary>
    /// Property name on the configuration type for the username.
    /// </summary>
    public string? UsernameProperty { get; set; }

    /// <summary>
    /// Property name on the configuration type for the password.
    /// </summary>
    public string? PasswordProperty { get; set; }
}
