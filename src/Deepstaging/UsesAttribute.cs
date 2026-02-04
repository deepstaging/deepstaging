namespace Deepstaging;

/// <summary>
/// Declares a dependency on an effects module for a Deepstaging runtime.
/// Apply multiple times to aggregate capabilities from different modules.
/// </summary>
/// <example>
/// <code>
/// [Runtime]
/// [Uses(typeof(EmailModule))]
/// [Uses(typeof(StorageModule))]
/// public partial class AppRuntime;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class UsesAttribute : Attribute
{
    /// <summary>
    /// Gets the effects module type this runtime depends on.
    /// Must be a partial class decorated with <see cref="EffectsModuleAttribute"/>.
    /// </summary>
    public Type ModuleType { get; }
    
    /// <summary>
    /// Creates a new Uses attribute declaring a module dependency.
    /// </summary>
    /// <param name="moduleType">
    /// The effects module type to include in the runtime.
    /// Must be a partial class with at least one <see cref="EffectsModuleAttribute"/>.
    /// </param>
    public UsesAttribute(Type moduleType)
    {
        ModuleType = moduleType;
    }
}
