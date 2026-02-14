// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Testing.Generators.Writers;

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
                .AddUsing("System")

                // Add a stub record, field, property, and builder methods for each capability
                .WithEach(model.Capabilities, (type, capability) => type
                    .WithMetadata("stub", new StubInfo(capability))
                    .ImplementCapability()
                    .AddCapabilityBuilder()
                    .AddStubBuilder()
                    .AddStubRecord())

                // Add static Create() factory method
                .AddMethod(MethodBuilder
                    .Parse($"public static {model.TestRuntimeType.CodeName} Create()")
                    .WithEach(model.Capabilities, (method, capability) => method
                        .AddParameter(capability.ParameterName, TypeRef.From(capability.DependencyType).Nullable(), pb => pb.WithDefaultValue("null")))
                    .WithBody(body => body
                        .AddStatement($"var runtime = new {model.TestRuntimeType.CodeName}()")
                        .WithEach(model.Capabilities, (bod, capability) => bod
                            .AddStatement($"runtime.{capability.ParameterName.ToBackingFieldName()} = {capability.ParameterName}"))
                        .AddReturn("runtime")))
                .Emit();
    }

    private static TypeBuilder AddCapabilityBuilder(this TypeBuilder type)
    {
        var capability = type.GetMetadata<StubInfo>("stub");

        return type
            .AddMethod(MethodBuilder
                .Parse($"public {type.Name} With{capability.PropertyName}({capability.DependencyType.CodeName} {capability.ParameterName})")
                .WithBody(body => body
                    .AddStatement($"{capability.FieldName} = {capability.ParameterName}")
                    .AddReturn("this")));
    }

    private static TypeBuilder AddStubBuilder(this TypeBuilder type)
    {
        var stub = type.GetMetadata<StubInfo>("stub");

        return type
            .AddMethod(MethodBuilder
                .Parse($"public {type.Name} With{stub.RecordName}()")
                .AddParameter("configure", stub.ConfigureDelegate)
                .WithBody(body => body
                    .AddStatement($"{stub.FieldName} = configure({stub.FieldName} as {stub.RecordName} ?? new {stub.RecordName}())")
                    .AddReturn("this")));
    }

    private static TypeBuilder ImplementCapability(this TypeBuilder type)
    {
        var stub = type.GetMetadata<StubInfo>("stub");

        var exceptionMessage =
            $"Service '{stub.PropertyName}' has not been configured on {type.Name}. " +
            $"Use .With{stub.PropertyName}() or Create({stub.ParameterName}:) to configure it.";

        return type
            .Implements(stub.Interface)
            .AddField(FieldBuilder
                .For(stub.FieldName, stub.NullableDependencyType))
            .AddProperty(PropertyBuilder
                .For(stub.PropertyName, stub.DependencyType.CodeName)
                .WithGetter($"{stub.FieldName} ?? throw new InvalidOperationException(\"{exceptionMessage}\")"));
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
        public TypeRef ConfigureDelegate => TypeRef.Func(RecordName, RecordName);
        public EquatableArray<CapabilityMethodModel> Methods => capability.Methods;
    }
}
