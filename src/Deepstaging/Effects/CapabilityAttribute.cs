// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects;

/// <summary>
/// Declares a raw capability dependency on the runtime without generating effect methods.
/// Use this for services that are accessed directly (e.g., configuration providers) rather than
/// lifted into the <c>Eff</c> effect system.
/// </summary>
/// <remarks>
/// Unlike <see cref="EffectsModuleAttribute"/>, this attribute does not generate effect wrapper methods
/// or OpenTelemetry instrumentation. It only generates the <c>IHas*</c> capability interface and
/// wires the dependency into the runtime.
/// </remarks>
/// <example>
/// <code>
/// [EffectsModule(typeof(INotifier))]
/// [Capability(typeof(IWorkshopConfigProvider))]
/// public sealed partial class RuntimeEffects;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class CapabilityAttribute : Attribute
{
    /// <summary>
    /// Gets the target type to expose as a runtime capability.
    /// </summary>
    public Type TargetType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CapabilityAttribute"/> class.
    /// </summary>
    /// <param name="targetType">The type to expose as a runtime capability.</param>
    public CapabilityAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}
