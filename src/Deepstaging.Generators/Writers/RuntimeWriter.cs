// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Generators.Writers;

/// <summary>
/// 
/// </summary>
public static class RuntimeWriter
{
    extension(RuntimeModel model)
    {
        /// <summary>
        /// 
        /// </summary>
        public OptionalEmit WriteRuntimeClass()
        {
            var constructor = ConstructorBuilder
                .For(model.RuntimeTypeName)
                .WithEach(model.Capabilities,
                    (b, capability) => b.AddParameter(capability.ParameterName, capability.DependencyType));

            return TypeBuilder
                .Parse($"{model.AccessibilityModifier} partial class {model.RuntimeTypeName}")
                .AddUsing("System.Threading.Tasks")
                .Implements([..model.Capabilities.Select(x => x.Interface)])
                .InNamespace(model.Namespace)
                .WithPrimaryConstructor(constructor)
                .WithEach(model.Capabilities, (builder, capability) => builder
                    .AddProperty(capability.PropertyName, capability.DependencyType, prop => prop
                        .WithGetter(capability.ParameterName)))
                .Emit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public OptionalEmit WriteRuntimeBootstrapperClass()
        {
            var boostrapMethod = MethodBuilder
                .Parse($"public static IServiceCollection Add{model.RuntimeTypeName}(this IServiceCollection services)")
                .AddUsings("Microsoft.Extensions.DependencyInjection", "System.Threading.Tasks")
                .WithBody(body => body
                    .AddStatement($"services.AddScoped<{model.RuntimeType}>()")
                    .AddReturn("services"));

            return TypeBuilder.Parse($"public static class {model.RuntimeTypeName}Bootstrapper")
                .InNamespace(model.Namespace)
                .AddMethod(boostrapMethod)
                .Emit();
        }
    }
}