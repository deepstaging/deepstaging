namespace Deepstaging.Effects.Queries.Attributes;

/// <summary>
/// 
/// </summary>
public sealed record UsesAttributeQuery(AttributeData AttributeData)
{
    /// <summary>
    /// 
    /// </summary>
    public ValidSymbol<INamedTypeSymbol> ModuleType => AttributeData
        .GetConstructorArgument<INamedTypeSymbol>(0)
        .Map(symbol => symbol.AsValidNamedType())
        .OrThrow("UsesAttribute must have a valid module type as its first constructor argument.");
}