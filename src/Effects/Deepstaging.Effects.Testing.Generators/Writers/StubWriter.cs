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
        var methods = stub.DependencyType.QueryMethods().GetAll();

        return TypeBuilder
            .Parse($"public partial record {stub.RecordName} : {stub.DependencyType}")
            .IfNot(string.IsNullOrEmpty(stub.DependencyType.Namespace), b => b.AddUsing(stub.DependencyType.Namespace!))
            .WithEach(methods, (builder, method) => builder
                .AddProperty($"On{method.Name}", method.AsDelegate().Nullable(), p => p.WithAutoPropertyAccessors().WithInitOnlySetter())
                .AddMethod(MethodBuilder
                    .Parse($"public {method.ReturnType} {method.Name}()")
                    .WithEach(method.Parameters, (m, param) => m
                        .AddParameter(param.ParameterName, param.Type)
                    )
                    .WithExpressionBody(TypeRef
                        .From($"On{method}")
                        .Invoke([..method.Parameters.Select(x => x.ParameterName)])
                        .OrDefault(method.ReturnType switch
                        {
                            { IsNonGenericTask: true } => TypeRef.CompletedTask,
                            { IsNonGenericValueTask: true } => "default",
                            { IsGenericTask: true } => $"global::System.Threading.Tasks.Task.FromResult<{method.ReturnType.InnerTaskType}>(default!)",
                            { IsGenericValueTask: true } => $"new global::System.Threading.Tasks.ValueTask<{method.ReturnType.InnerTaskType}>(default!)",
                            _ => "default!"
                        }))
                ));
    }

    private static TypeRef AsDelegate(this ValidSymbol<IMethodSymbol> method) => method switch
    {
        { ReturnsVoid: true } => TypeRef.Action([..method.Parameters.Select(param => TypeRef.From(param.Type))]),
        _ => TypeRef.Func([..method.Parameters.Select(param => TypeRef.From(param.Type)), TypeRef.From(method.ReturnType)])
    };
}
