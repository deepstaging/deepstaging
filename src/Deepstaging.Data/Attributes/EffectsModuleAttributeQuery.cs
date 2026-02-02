using Deepstaging.Data.Models;

namespace Deepstaging.Data.Attributes;

using Attr = EffectsModuleAttribute;

/// <summary>
/// 
/// </summary>
public sealed record EffectsModuleAttributeQuery(AttributeData AttributeData)
{
    /// <summary>
    /// 
    /// </summary>
    public string Name => AttributeData
        .GetNamedArgument<string>(nameof(Attr.Name))
        .OrDefault(() => TargetType switch
        {
            { IsInterface: true, Name: { Length: > 1 } name } when name[0] == 'I' => name.Substring(1),
            _ => TargetType.Name
        });

    /// <summary>
    /// 
    /// </summary>
    public ValidSymbol<INamedTypeSymbol> TargetType => AttributeData
        .GetConstructorArgument<INamedTypeSymbol>(0)
        .Map(symbol => symbol.AsValidNamedType())
        .OrThrow("GetEffectsModuleAttribute must have a valid target type as its first constructor argument.");


    /// <summary>
    /// 
    /// </summary>
    public bool Instrumented => AttributeData
        .GetNamedArgument<bool>(nameof(Attr.Instrumented))
        .OrDefault(true);

    /// <summary>
    /// 
    /// </summary>
    public ImmutableArray<string> IncludeOnly => AttributeData
        .GetNamedArgument<string[]>(nameof(Attr.IncludeOnly))
        .Map(ImmutableArray.Create)
        .OrDefault([]);

    /// <summary>
    /// 
    /// </summary>
    public ImmutableArray<string> Exclude => AttributeData
        .GetNamedArgument<string[]>(nameof(Attr.Exclude))
        .Map(ImmutableArray.Create)
        .OrDefault([]);
}