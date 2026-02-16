// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Effects;

using LocalRefs;

/// <summary>
/// Emits code for standalone [EffectsModule] decorated classes.
/// </summary>
public static partial class EffectsModuleWriter
{
    extension(EffectsModuleModel model)
    {
        /// <summary>
        /// Emits the capability interface (e.g., IHasAppDbContext).
        /// </summary>
        public OptionalEmit WriteCapabilityInterface() =>
            TypeBuilder
                .Interface(model.Capability.Interface)
                .InNamespace(model.Namespace)
                .AddUsings("Deepstaging.Effects", TaskRefs.Namespace)
                .AddProperty(model.Capability.PropertyName, model.TargetType, builder => builder.AsReadOnly())
                .WithXmlDoc(xml => xml
                    .WithSummary($"Runtime capability interface for the {model.Capability.DependencyType.Name} dependency of the {model.Name} effects module.")
                    .AddSeeAlso($"global::{model.Namespace}.{model.EffectsContainerName}.{model.Name}")
                ).Emit();

        /// <summary>
        /// Emits the effects module with all effect methods.
        /// </summary>
        public OptionalEmit WriteEffectsModule()
        {
            var module = TypeBuilder
                .Parse($"public static partial class {model.Name}")
                // TODO: Add flagged Effects enum to this class that records all effects in this module.
                //       This allows users to easily specify sets of effects for registration.
                .AddInstrumentationActivitySource(model)
                .AddEffectMethods(model)
                .AddDbContextEffects(model)
                .WithXmlDoc(model.XmlDocumentation);

            return TypeBuilder
                .Parse($"public static partial class {model.EffectsContainerName}")
                .AddUsings(SystemRefs.Namespace, "Deepstaging.Effects")
                .AddUsings(DiagnosticsRefs.Namespace, CollectionRefs.Namespace)
                .AddUsings(LanguageExtRefs.Namespace, LanguageExtRefs.EffectsNamespace, LanguageExtRefs.PreludeNamespace)
                .InNamespace(model.Namespace)
                .WithXmlDoc($"Container class for the <c>{model.Name}</c> effects module and its nested entity set effect classes.")
                .AddNestedType(module)
                .Emit();
        }
    }

    internal static TypeBuilder AddInstrumentationActivitySource(this TypeBuilder builder, EffectsModuleModel module) => builder
        .If(module.Instrumented, b => b
            .AddUsing(DiagnosticsRefs.Namespace)
            .AddField(FieldBuilder.Parse($"private static readonly {DiagnosticsRefs.Namespace} ActivitySource")
                .WithInitializer($"""new("{module.Namespace}.{module.Name}", "1.0.0")""")));
}