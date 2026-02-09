// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.HttpClient.Projection.Models;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;

namespace Deepstaging.HttpClient.Projection.Attributes;

/// <summary>
/// Query wrapper for HTTP verb attributes (Get, Post, Put, Patch, Delete).
/// </summary>
public sealed record HttpVerbAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// The request path from the attribute constructor.
    /// </summary>
    public string Path => ConstructorArg<string>(0).OrDefault("");
    
    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public HttpVerb Verb => AttributeData.AttributeClass?.Name switch
    {
        nameof(GetAttribute) => HttpVerb.Get,
        nameof(PostAttribute) => HttpVerb.Post,
        nameof(PutAttribute) => HttpVerb.Put,
        nameof(PatchAttribute) => HttpVerb.Patch,
        nameof(DeleteAttribute) => HttpVerb.Delete,
        _ => throw new InvalidOperationException("Unknown HTTP verb attribute.")
    };
}
