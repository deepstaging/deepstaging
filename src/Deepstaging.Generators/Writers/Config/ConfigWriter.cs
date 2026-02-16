// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Config;

/// <summary>
/// Generates the partial config provider class, interface, and DI extension method.
/// </summary>
public static class ConfigWriter
{
    extension(ConfigModel model)
    {
        /// <summary>
        /// Generates the config provider class, interface, and DI registration extension.
        /// </summary>
        public OptionalEmit WriteConfigClass()
        {
            var interfaceName = $"I{model.TypeName}";

            var @interface = TypeBuilder
                .Interface(interfaceName)
                .InNamespace(model.Namespace)
                .WithEach(model.ExposedConfigurationTypes, (builder, configType) => builder
                    .AddProperty(configType.Type.Name, configType.Type.CodeName, prop => prop
                        .WithAutoPropertyAccessors()
                        .WithXmlDoc($"Gets the <see cref=\"{configType.Type.Name}\"/> configuration.")))
                .WithXmlDoc($"Interface for the <c>{model.TypeName}</c> configuration provider.")
                .Emit();

            var type = TypeBuilder
                .Class(model.TypeName)
                .AsPartial()
                .WithAccessibility(model.Accessibility)
                .Implements(interfaceName)
                .InNamespace(model.Namespace)
                .AddUsing(ConfigurationRefs.Namespace)
                .WithPrimaryConstructor(c => c
                    .AddParameter("configuration", ConfigurationRefs.IConfiguration))
                .AddField("_section", ConfigurationRefs.IConfigurationSection, f => f
                    .AsReadonly()
                    .WithInitializer($"configuration.GetSection(\"{model.Section}\")"))
                .WithEach(model.ExposedConfigurationTypes, (builder, configType) => builder
                    .AddProperty(configType.Type.Name, configType.Type.CodeName, prop => prop
                        .WithXmlDoc($"Gets the <see cref=\"{configType.Type.Name}\"/> configuration.")
                        .WithGetter($"_section.GetSection(\"{configType.Type.Name}\").Get<{configType.Type.CodeName}>()!")))
                .Emit();

            return @interface
                .Combine(type)
                .Combine(model.WriteExtensionsClass());
        }
    }

    /// <summary>
    /// Generates a static extension class with a DI registration method.
    /// </summary>
    private static OptionalEmit WriteExtensionsClass(this ConfigModel model) =>
        TypeBuilder
            .Parse($"public static class {model.TypeName}Extensions")
            .InNamespace(model.Namespace)
            .AddUsings(ConfigurationRefs.Namespace, DependencyInjectionRefs.Namespace)
            .AddMethod(MethodBuilder.Parse(
                    $"""
                     public static {DependencyInjectionRefs.IServiceCollection} Add{model.TypeName}(
                         this {DependencyInjectionRefs.IServiceCollection} services,
                         {ConfigurationRefs.IConfiguration} configuration
                     )
                     """)
                .WithBody(body => body
                    .AddStatement($"var provider = {model.TypeName.New("configuration")}")
                    .AddStatement($"services.AddSingleton<I{model.TypeName}>(provider)")
                    .AddReturn("services"))
                .WithXmlDoc(xml => xml
                    .WithSummary($"Registers <see cref=\"{model.TypeName}\"/> and <see cref=\"I{model.TypeName}\"/> with the service collection.")
                    .AddParam("services", "The service collection.")
                    .AddParam("configuration", "The application configuration.")
                    .WithReturns("The service collection for fluent chaining.")))
            .Emit();
}