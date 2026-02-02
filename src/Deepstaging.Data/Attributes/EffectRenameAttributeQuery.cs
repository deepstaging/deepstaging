namespace Deepstaging.Data.Attributes;

/// <summary>
/// 
/// </summary>
public record EffectRenameAttributeQuery(AttributeData AttributeData)
{
    /// <summary>
    /// 
    /// </summary>
    public string GroupName => AttributeData
        .GetConstructorArgument<string>(0)
        .OrThrow("EffectRenameAttribute must have a valid group name as its first constructor argument.");

    /// <summary>
    /// 
    /// </summary>
    public string MethodName => AttributeData
        .GetConstructorArgument<string>(1)
        .OrThrow("EffectRenameAttribute must have a valid method name as its second constructor argument.");

    /// <summary>
    /// 
    /// </summary>
    public string EffectName => AttributeData
        .GetConstructorArgument<string>(2)
        .OrThrow("EffectRenameAttribute must have a valid effect name as its third constructor argument.");
}