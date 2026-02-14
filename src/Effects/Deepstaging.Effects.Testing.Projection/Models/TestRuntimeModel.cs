// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Effects.Projection.Models;

namespace Deepstaging.Effects.Testing.Projection.Models;

/// <summary>
/// Model for the test runtime class discovered via <see cref="TestRuntimeAttribute{TRuntime}"/>.
/// Contains the information needed to generate a test-friendly runtime implementation.
/// </summary>
[PipelineModel]
public sealed record TestRuntimeModel
{
    /// <summary>
    ///  The production runtime type that this test runtime is based on, referenced from the attribute's type argument.
    /// </summary>
    public required TypeSnapshot RuntimeType { get; init; }

    /// <summary>
    /// The name of the test runtime class (e.g., "TestAppRuntime").
    /// </summary>
    public required TypeSnapshot TestRuntimeType { get; init; }

    /// <summary>
    /// The namespace of the test runtime class.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The accessibility modifier as a string (e.g., "public", "internal").
    /// </summary>
    public required string AccessibilityModifier { get; init; }

    /// <summary>
    /// The capabilities discovered from the production runtime's [Uses] modules.
    /// </summary>
    public EquatableArray<RuntimeCapabilityModel> Capabilities { get; init; } = [];
}
