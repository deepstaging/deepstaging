// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Testing.Generators.Writers;

/// <summary>
/// Provides extension methods for generating inner stub classes for each capability
/// on a test runtime. Each stub implements the capability interface with configurable
/// <c>Func&lt;&gt;</c> delegate properties for each method.
/// </summary>
public static class StubWriter
{
    extension(TypeBuilder type)
    {
        /// <summary>
        /// Generates an inner stub record for a capability interface.
        /// The stub implements the interface with configurable delegate properties.
        /// </summary>
        /// <returns>A <see cref="TypeBuilder"/> with the inner stub record.</returns>
        public TypeBuilder AddStubRecord()
        {
            var stub = type.GetMetadata<TestRuntimeWriter.StubInfo>("stub");
            return type.AddNestedType(stub.CreateStubType());
        }
    }

    private static TypeBuilder CreateStubType(this TestRuntimeWriter.StubInfo stub)
    {
        return TypeBuilder
            .Parse($"public partial record {stub.RecordName} : {stub.DependencyType.CodeName}")
            .IfNot(string.IsNullOrEmpty(stub.DependencyType.Namespace), b => b.AddUsing(stub.DependencyType.Namespace!))
            .WithEach(stub.Methods, (builder, method) => builder
                .AddProperty($"On{method.Name}", $"{method.DelegateType}?", p => p.WithAutoPropertyAccessors().WithInitOnlySetter())
                .AddMethod(MethodBuilder
                    .Parse($"public {method.ReturnType} {method.Name}()")
                    .WithEach(method.Parameters, (m, param) => m
                        .AddParameter(param.Name, param.Type)
                    )
                    .WithExpressionBody(TypeRef
                        .From($"On{method.Name}")
                        .Invoke([..method.Parameters.Select(x => x.Name)])
                        .OrDefault(method switch
                        {
                            { IsNonGenericTask: true } => TypeRef.CompletedTask,
                            { IsNonGenericValueTask: true } => "default",
                            { IsGenericTask: true } => $"global::System.Threading.Tasks.Task.FromResult<{method.InnerTaskType}>(default!)",
                            { IsGenericValueTask: true } => $"new global::System.Threading.Tasks.ValueTask<{method.InnerTaskType}>(default!)",
                            _ => "default!"
                        }))
                ));
    }
}
