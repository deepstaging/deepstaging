namespace Deepstaging.Data.Models;

/// <summary>
/// Represents a parameter for an effect method.
/// </summary>
public sealed record EffectParameterModel
{
    /// <summary>
    /// The fully qualified type of the parameter.
    /// </summary>
    public required string Type { get; init; }
    
    /// <summary>
    /// The parameter name.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Whether this parameter has a default value.
    /// </summary>
    public bool HasDefaultValue { get; init; }
    
    /// <summary>
    /// The default value expression (if HasDefaultValue is true).
    /// </summary>
    public string? DefaultValue { get; init; }
}
