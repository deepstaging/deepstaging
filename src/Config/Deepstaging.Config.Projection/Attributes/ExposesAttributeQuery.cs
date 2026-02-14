// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Config.Projection.Attributes;

/// <summary>
/// A queryable wrapper over <see cref="ExposesAttribute{T}"/> data.
/// Provides access to the configuration type exposed by this attribute.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record ExposesAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// Gets the configuration type exposed by this attribute.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the type argument is not valid.</exception>
    public ValidSymbol<INamedTypeSymbol> ConfigurationType =>
        TypeArg(0).OrThrow("ExposesAttribute must have a valid type argument.").AsValidNamedType();
}
