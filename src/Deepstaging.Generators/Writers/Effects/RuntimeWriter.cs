// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Effects;

/// <summary>
/// Generates the partial runtime class with capability properties and a primary constructor.
/// </summary>
public static class RuntimeWriter
{
    private const string LoggerFactoryInterface = "global::Deepstaging.Effects.IHasLoggerFactory";

    extension(RuntimeModel model)
    {
        /// <summary>
        /// Generates the partial runtime class with capability properties and a primary constructor.
        /// </summary>
        public OptionalEmit WriteRuntimeClass()
        {
            var constructor = ConstructorBuilder
                .For(model.RuntimeTypeName)
                .WithEach(model.Capabilities, (b, capability) => b
                    .AddParameter(capability.ParameterName, capability.DependencyType.CodeName))
                .If(model.HasInstrumentedModules, b => b
                    .AddParameter("loggerFactory", type: LoggingTypes.ILoggerFactory.Nullable(), p => p.WithDefaultValue("null")));

            return TypeBuilder
                .Parse($"{model.AccessibilityModifier} partial class {model.RuntimeTypeName}")
                .AddUsing(TaskTypes.Namespace)
                .WithXmlDoc("Runtime class that implements capability interfaces and provides access to effect dependencies.")
                .Implements(model.CapabilitiesInterfaceName)
                .InNamespace(model.Namespace)
                .WithPrimaryConstructor(constructor)
                .WithEach(model.Capabilities, (builder, capability) => builder
                    .AddProperty(capability.PropertyName, capability.DependencyType.CodeName, prop => prop
                        .WithXmlDoc($"Gets the <c>{capability.DependencyType.Name}</c> dependency for this runtime.")
                        .WithGetter(capability.ParameterName)))
                .If(model.HasInstrumentedModules, b => b
                    .AddUsing(LoggingTypes.Namespace)
                    .Implements(LoggerFactoryInterface)
                    .AddProperty("LoggerFactory", LoggingTypes.ILoggerFactory, prop => prop
                        .WithXmlDoc("Gets the logger factory for structured logging of instrumented effect methods.")
                        .WithGetter("loggerFactory!")))
                .Emit();
        }

        /// <summary>
        /// Generates the capabilities interface combining all IHas* interfaces for this runtime.
        /// </summary>
        public OptionalEmit WriteRuntimeCapabilitiesInterface() =>
            TypeBuilder
                .Interface(model.CapabilitiesInterfaceName)
                .InNamespace(model.Namespace)
                .Implements([..model.Capabilities.Select(x => x.Interface)])
                .WithXmlDoc(
                    $"""
                     Composite capability interface for the {model.RuntimeTypeName} runtime. 
                     Combines all IHas* interfaces for use as a generic constraint.
                     """)
                .Emit();
    }
}