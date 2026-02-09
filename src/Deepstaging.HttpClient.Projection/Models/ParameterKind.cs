// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.HttpClient.Projection.Models;

/// <summary>
/// Indicates how a parameter should be bound in the HTTP request.
/// </summary>
public enum ParameterKind
{
    /// <summary>
    /// Parameter binding was not explicitly specified. Will be inferred.
    /// </summary>
    None,

    /// <summary>
    /// Parameter is substituted into the URL path.
    /// </summary>
    Path,

    /// <summary>
    /// Parameter is added to the query string.
    /// </summary>
    Query,

    /// <summary>
    /// Parameter is added as an HTTP header.
    /// </summary>
    Header,

    /// <summary>
    /// Parameter is serialized as the request body.
    /// </summary>
    Body
}
