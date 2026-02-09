// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Generators.Writers;

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
        public OptionalEmit WriteCapabilityInterface()
        {
            return TypeBuilder
                .Interface(model.Capability.Interface)
                .InNamespace(model.Namespace)
                .AddUsings("Deepstaging", "Deepstaging.Runtime", "System.Threading.Tasks")
                .AddProperty(model.Capability.PropertyName, model.TargetType, builder => builder.AsReadOnly())
                .WithXmlDoc(xml => xml
                    .WithSummary(
                        $"Runtime capability interface for the {model.Capability.DependencyType} dependency of the {model.Name} effects module.")
                    .AddSeeAlso($"global::{model.Namespace}.{model.EffectsContainerName}.{model.Name}")
                ).Emit();
        }

        /// <summary>
        /// Emits the effects module with all effect methods.
        /// </summary>
        public OptionalEmit WriteEffectsModule(EmitOptions? options = null)
        {
            var usings = new[]
            {
                "Deepstaging",
                "Deepstaging.Runtime",
                "LanguageExt",
                "LanguageExt.Effects",
                "Microsoft.Extensions.Logging",
                "System.Diagnostics",
                "System.Linq.Expressions",
                "System",
                "System.Collections.Generic",
                "System.Threading",
                "System.Threading.Tasks",
                "static LanguageExt.Prelude"
            };

            var module = TypeBuilder.Parse($"public static partial class {model.Name}")
                .AddInstrumentationActivitySource(model)
                .AddEffectMethods(model)
                .AddDbContextEffects(model)
                .WithXmlDoc(model.XmlDocumentation);

            return TypeBuilder.Parse($"public static partial class {model.EffectsContainerName}")
                .AddUsings(usings)
                .InNamespace(model.Namespace)
                .AddNestedType(module)
                .Emit(options ?? EmitOptions.Default);
        }
    }

    internal static TypeBuilder AddInstrumentationActivitySource(this TypeBuilder builder, EffectsModuleModel module)
    {
        return builder.If(module.Instrumented, b => b
            .AddUsing("System.Diagnostics")
            .AddField(FieldBuilder.Parse("private static readonly ActivitySource ActivitySource")
                .WithInitializer($"""new("{module.Namespace}.{module.Name}", "1.0.0")""")));
    }
}