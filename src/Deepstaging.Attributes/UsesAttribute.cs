namespace Deepstaging;

 
/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class UsesAttribute : Attribute
{
    /// <summary>
    /// </summary>
    public Type ModuleType { get; }
    
    /// <summary>
    /// </summary>
    public UsesAttribute(Type moduleType)
    {
        ModuleType = moduleType;
    }
}
