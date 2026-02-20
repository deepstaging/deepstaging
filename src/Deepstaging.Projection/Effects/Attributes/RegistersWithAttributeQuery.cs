// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects.Attributes;

/// <summary>
/// A queryable wrapper over <see cref="RegistersWithAttribute"/> data.
/// Provides access to the referenced DI registration method and its validation state.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record RegistersWithAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// Gets the name of the registration method as specified in the attribute.
    /// </summary>
    public string MethodName => ConstructorArg<string>(0)
        .OrThrow($"{nameof(RegistersWithAttribute)} must have a method name as its first constructor argument.");

    /// <summary>
    /// Gets whether the referenced method can be resolved on the containing type.
    /// </summary>
    public bool HasValidMethod(ValidSymbol<INamedTypeSymbol> containingType) =>
        ResolveMethod(containingType) is not null;

    /// <summary>
    /// Resolves the referenced static method on the containing type.
    /// Returns null if no matching method is found.
    /// </summary>
    public IMethodSymbol? ResolveMethod(ValidSymbol<INamedTypeSymbol> containingType) =>
        containingType.Value
            .GetMembers(MethodName)
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsStatic && m.IsExtensionMethod);

    /// <summary>
    /// Validates that the method is a valid DI registration method.
    /// A valid method must be static, an extension method, and its first parameter must be
    /// <c>IServiceCollection</c>, <c>WebApplicationBuilder</c>, or <c>IHostApplicationBuilder</c>.
    /// </summary>
    public bool IsValidRegistrationMethod(ValidSymbol<INamedTypeSymbol> containingType)
    {
        var method = ResolveMethod(containingType);
        if (method is null || method.Parameters.Length == 0)
            return false;

        var firstParamType = method.Parameters[0].Type.Name;
        return firstParamType is "IServiceCollection" or "WebApplicationBuilder" or "IHostApplicationBuilder";
    }
}
