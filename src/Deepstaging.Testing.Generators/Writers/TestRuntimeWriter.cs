// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Testing.Generators.Writers;

/// <summary>
/// Provides extension methods for generating test runtime source code from a <see cref="TestRuntimeModel"/>.
/// </summary>
public static class TestRuntimeWriter
{
    extension(TestRuntimeModel model)
    {
        /// <summary>
        /// Generates the partial test runtime class with capability properties,
        /// a builder pattern via <c>With*()</c> methods, and a static <c>Create()</c> factory.
        /// </summary>
        public OptionalEmit WriteTestRuntimeClass() =>
            TypeBuilder
                .Parse($"{model.AccessibilityModifier} partial class {model.TestRuntimeType.Name}")
                .InNamespace(model.Namespace)
                .AddUsing(SystemRefs.Namespace)
                .Implements(model.CapabilitiesInterfaceFullName)
                .Implements($"global::Deepstaging.ITestRuntime<{model.TestRuntimeType.CodeName}>")
                .WithXmlDoc(xml => xml
                    .WithSummary($"Test runtime for <c>{model.TestRuntimeType.Name}</c> with configurable capability stubs and a builder pattern."))

                // Add a stub record, field, property, and builder methods for each capability
                .WithEach(model.Capabilities, (type, capability) => type
                    .WithMetadata("stub", new StubInfo(capability))
                    .ImplementCapability()
                    .AddCapabilityBuilder()
                    .If(capability.DependencyType.IsInterface, b => b
                        .AddStubRecord()))

                // Add static Create() factory method (implements ITestRuntime<TSelf>)
                .AddMethod(MethodBuilder
                    .Parse($"public static {model.TestRuntimeType.CodeName} Create()")
                    .WithBody(body => body
                        .AddReturn($"new {model.TestRuntimeType.CodeName}()"))
                    .WithXmlDoc(xml => xml
                        .WithSummary($"Creates a new <c>{model.TestRuntimeType.Name}</c> instance with all capabilities unconfigured. Use <c>.With*()</c> methods to configure.")
                        .WithReturns("A new test runtime instance.")))
                .Emit();
    }

    private static TypeBuilder AddCapabilityBuilder(this TypeBuilder type)
    {
        var capability = type.GetMetadata<StubInfo>("stub");

        return type.AddMethod(MethodBuilder
            .Parse($"public {type.Name} With{capability.PropertyName}({capability.DependencyType.CodeName} {capability.ParameterName})")
            .WithBody(body => body
                .AddStatement($"{capability.FieldName} = {capability.ParameterName}")
                .AddReturn("this"))
            .WithXmlDoc(xml => xml
                .WithSummary($"Configures the <c>{capability.PropertyName}</c> capability with the specified implementation.")
                .AddParam(capability.ParameterName, $"The {capability.DependencyType.Name} implementation to use.")
                .WithReturns("This test runtime instance for fluent chaining.")));
    }

    private static TypeBuilder ImplementCapability(this TypeBuilder type)
    {
        var stub = type.GetMetadata<StubInfo>("stub");

        var exceptionMessage =
            $"Service '{stub.PropertyName}' has not been configured on {type.Name}. " +
            $"Use .With{stub.PropertyName}() to configure it.";

        return type
            .AddField(FieldBuilder
                .For(stub.FieldName, stub.NullableDependencyType))
            .AddProperty(PropertyBuilder
                .For(stub.PropertyName, stub.DependencyType.CodeName)
                .WithXmlDoc(xml => xml
                    .WithSummary($"Gets the <c>{stub.DependencyType.Name}</c> capability, or throws if not configured.")
                    .AddException(ExceptionRefs.InvalidOperation, "Thrown if the capability has not been configured."))
                .WithGetter($"{stub.FieldName} ?? throw new {ExceptionRefs.InvalidOperation}(\"{exceptionMessage}\")"));
    }

    internal class StubInfo(RuntimeCapabilityModel capability)
    {
        public string Interface => capability.Interface;
        public string RecordName => $"Stub{capability.PropertyName}";
        public string FieldName => capability.ParameterName.ToBackingFieldName();
        public string PropertyName => capability.PropertyName;
        public string ParameterName => capability.ParameterName;
        public TypeSnapshot DependencyType => capability.DependencyType;
        public string NullableDependencyType => $"{capability.DependencyType.CodeName}?";
        public TypeRef ConfigureDelegate => DelegateRefs.Func(RecordName, RecordName);
        public EquatableArray<CapabilityMethodModel> Methods => capability.Methods;
    }
}