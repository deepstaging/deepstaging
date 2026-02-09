// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.HttpClient.Projection.Models;

/// <summary>
/// HTTP verb type.
/// </summary>
public enum HttpVerb
{
    /// <summary>HTTP GET method.</summary>
    Get,
    /// <summary>HTTP POST method.</summary>
    Post,
    /// <summary>HTTP PUT method.</summary>
    Put,
    /// <summary>HTTP PATCH method.</summary>
    Patch,
    /// <summary>HTTP DELETE method.</summary>
    Delete
}
