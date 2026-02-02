namespace Deepstaging.Data.Models;

/// <summary>
/// Model for the runtime class discovered via [Runtime] attribute.
/// </summary>
public sealed record RuntimeModel
{
    /// <summary>
    /// The name of the runtime class (e.g., "AppRuntime").
    /// </summary>
    public required string ClassName { get; init; }

    /// <summary>
    /// The namespace of the runtime class (e.g., "MyApp").
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The accessibility modifier (e.g., "public", "internal").
    /// </summary>
    public required Accessibility Accessibility { get; init; }

    /// <summary>
    /// The accessibility modifier as a string (e.g., "public", "internal").
    /// </summary>
    public required string AccessibilityModifier { get; init; }

    /// <summary>
    /// Names of capability interfaces this runtime implements (e.g., ["IHasEmail", "IHasDatabase"]).
    /// </summary>
    public ImmutableArray<string> CapabilityInterfaces { get; init; } = [];
}