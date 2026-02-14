// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;

namespace Deepstaging.HttpClient.Projection.Attributes;

/// <summary>
/// Query wrapper for the HttpClientAttribute.
/// </summary>
public sealed record HttpClientAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// The configuration type, if using the generic HttpClientAttribute{T}.
    /// </summary>
    public OptionalSymbol<INamedTypeSymbol> ConfigurationType =>
        AttributeData.Query()
            .GetTypeArgument(0)
            .Map(symbol => symbol.AsNamedType())
            .OrDefault(OptionalSymbol<INamedTypeSymbol>.Empty);

    /// <summary>
    /// Optional base address for all requests.
    /// </summary>
    public string? BaseAddress => NamedArg<string>("BaseAddress").OrNull();
}
