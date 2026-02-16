// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Testing.Projection.Attributes;

/// <summary>
/// A queryable wrapper over <see cref="TestRuntimeAttribute{TRuntime}"/> data.
/// Provides access to the referenced production runtime type.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record TestRuntimeAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// Gets the production runtime type referenced by this attribute's type argument.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the runtime type is not valid.</exception>
    public ValidSymbol<INamedTypeSymbol> RuntimeType => TypeArg(0)
        .OrThrow($"{nameof(TestRuntimeAttribute<>)} must have a valid runtime type argument.")
        .AsValidNamedType();
}
