// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.HttpClient.Projection.Models;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;

namespace Deepstaging.HttpClient.Projection.Attributes;

/// <summary>
/// Query wrapper for parameter attributes (Path, Query, Header, Body).
/// </summary>
public sealed record ParameterAttributeQuery(AttributeData AttributeData, ParameterKind Kind) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// The custom name (for Header or Query attributes).
    /// </summary>
    public string? Name => Kind switch
    {
        ParameterKind.Header => ConstructorArg<string>(0).OrNull(),
        ParameterKind.Query => ConstructorArg<string>(0).OrNull(),
        _ => null
    };
}
