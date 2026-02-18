// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects.Models;

/// <summary>
/// Model for the runtime class discovered via [Runtime] attribute.
/// </summary>
[PipelineModel]
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
    public EquatableArray<RuntimeCapabilityModel> Capabilities { get; init; } = [];

    /// <summary>
    /// OpenTelemetry ActivitySource names from instrumented modules (e.g., ["MyApp.EmailService", "MyApp.Database"]).
    /// </summary>
    public EquatableArray<string> ActivitySources { get; init; } = [];

    /// <summary>
    /// Whether any modules are instrumented, indicating that logging support should be available.
    /// </summary>
    public bool HasInstrumentedModules { get; init; }

    /// <summary>
    /// The name of the generated composite capabilities interface (e.g., "IRuntimeCapabilities").
    /// </summary>
    public string CapabilitiesInterfaceName => $"I{RuntimeTypeName}Capabilities";
}

/// <summary>
/// Model for a capability dependency required by the runtime.
/// </summary>
[PipelineModel]
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
    /// Pipeline-safe snapshot of the dependency type required by this capability.
    /// </summary>
    public required TypeSnapshot DependencyType { get; init; }

    /// <summary>
    /// Pre-extracted methods from the dependency type, used by StubWriter for test runtime generation.
    /// </summary>
    public EquatableArray<CapabilityMethodModel> Methods { get; init; } = [];

    /// <summary>
    /// Whether this capability is standalone (from <c>[Capability]</c>) rather than from <c>[EffectsModule]</c>.
    /// Standalone capabilities need their <c>IHas*</c> interface emitted by the runtime pipeline.
    /// </summary>
    public bool IsStandalone { get; init; }
}

/// <summary>
/// Pipeline-safe model for a method on a capability interface.
/// </summary>
[PipelineModel]
public sealed record CapabilityMethodModel
{
    /// <summary>The method name.</summary>
    public required string Name { get; init; }

    /// <summary>The fully qualified return type.</summary>
    public required string ReturnType { get; init; }

    /// <summary>Whether the method returns void.</summary>
    public required bool ReturnsVoid { get; init; }

    /// <summary>Whether the return type is a non-generic Task.</summary>
    public required bool IsNonGenericTask { get; init; }

    /// <summary>Whether the return type is a non-generic ValueTask.</summary>
    public required bool IsNonGenericValueTask { get; init; }

    /// <summary>Whether the return type is a generic Task.</summary>
    public required bool IsGenericTask { get; init; }

    /// <summary>Whether the return type is a generic ValueTask.</summary>
    public required bool IsGenericValueTask { get; init; }

    /// <summary>The inner type argument of Task/ValueTask, if generic.</summary>
    public string? InnerTaskType { get; init; }

    /// <summary>The method parameters.</summary>
    public required EquatableArray<CapabilityParameterModel> Parameters { get; init; }

    /// <summary>Pre-computed delegate type string (e.g., "System.Func&lt;int, string&gt;").</summary>
    public required TypeRef DelegateType { get; init; }
}

/// <summary>
/// Pipeline-safe model for a parameter on a capability method.
/// </summary>
[PipelineModel]
public sealed record CapabilityParameterModel
{
    /// <summary>The parameter name.</summary>
    public required string Name { get; init; }

    /// <summary>The fully qualified parameter type.</summary>
    public required string Type { get; init; }
}