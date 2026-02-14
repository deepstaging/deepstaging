// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Effects.Projection.Models;

/// <summary>
/// Model for the runtime class discovered via [Runtime] attribute.
/// </summary>
public sealed record RuntimeModel
{
    /// <summary>
    /// The name of the runtime class (e.g., "AppRuntime").
    /// </summary>
    public required string RuntimeTypeName { get; init; }

    /// <summary>
    ///  The fully qualified type name of the runtime class (e.g., "MyApp.AppRuntime").
    /// </summary>
    public required string RuntimeType { get; init; }

    /// <summary>
    /// The namespace of the runtime class (e.g., "MyApp").
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The accessibility modifier as a string (e.g., "public", "internal").
    /// </summary>
    public required string AccessibilityModifier { get; init; }

    /// <summary>
    /// Names of capability interfaces this runtime implements (e.g., ["IHasEmail", "IHasDatabase"]).
    /// </summary>
    public ImmutableArray<RuntimeCapabilityModel> Capabilities { get; init; } = [];
}

/// <summary>
/// Model for a capability dependency required by the runtime.
/// </summary>
public sealed record RuntimeCapabilityModel
{
    /// <summary>
    /// The name of the capability interface (e.g., "IHasEmail").
    /// </summary>
    public required string Interface { get; init; }

    /// <summary>
    /// The property name for the capability interface (e.g., "EmailService").
    /// </summary>
    public required string PropertyName { get; init; }

    /// <summary>
    /// The constructor parameter name for the capability (e.g., "emailService").
    /// </summary>
    public required string ParameterName { get; init; }

    /// <summary>
    ///  The symbol of the dependency type required by this capability (e.g., the symbol for "MyApp.Services.EmailService").
    /// </summary>
    public required ValidSymbol<INamedTypeSymbol> DependencyType { get; set; }
}
