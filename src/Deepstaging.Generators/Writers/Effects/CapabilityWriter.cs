// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Effects;

using LocalRefs;

/// <summary>
/// Emits capability interfaces for standalone [Capability] declarations.
/// </summary>
public static class CapabilityWriter
{
    extension(RuntimeCapabilityModel model)
    {
        /// <summary>
        /// Emits the capability interface (e.g., IHasWorkshopConfigProvider) for a standalone capability.
        /// </summary>
        public OptionalEmit WriteCapabilityInterface(string ns) =>
            TypeBuilder
                .Interface(model.Interface)
                .InNamespace(ns)
                .AddUsings("Deepstaging.Effects", TaskRefs.Namespace)
                .AddProperty(model.PropertyName, model.DependencyType.CodeName, builder => builder.AsReadOnly())
                .WithXmlDoc(xml => xml
                    .WithSummary($"Runtime capability interface for the {model.DependencyType.Name} dependency."))
                .Emit();
    }
}
