// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Testing.Generators.Writers;

using Roslyn.Expressions;

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

            return type
                .AddStubBuilder()
                .AddNestedType(stub.CreateStubType());
        }
    }


    /// <summary>
    /// Generates a builder method for configuring the stub of a capability interface.
    /// </summary>
    /// <returns>A <see cref="TypeBuilder"/> with the stub builder method.</returns>
    private static TypeBuilder AddStubBuilder(this TypeBuilder type)
    {
        var stub = type.GetMetadata<TestRuntimeWriter.StubInfo>("stub");

        return type.AddMethod(MethodBuilder
                .Parse($"public {type.Name} With{stub.RecordName}()")
                .AddParameter("configure", stub.ConfigureDelegate)
                .WithBody(body => body
                    .AddStatement($"{stub.FieldName} = configure({TypeRef.From(stub.FieldName).As(stub.RecordName)} ?? new {stub.RecordName}())")
                    .AddReturn("this")))
            .WithXmlDoc(xml => xml
                .WithSummary($"Configures the <c>{stub.RecordName}</c> stub using a builder delegate.")
                .AddParam("configure", "A delegate that receives the current stub (or a new default) and returns the configured stub.")
                .WithReturns("This test runtime instance for fluent chaining."));
    }

    private static TypeBuilder CreateStubType(this TestRuntimeWriter.StubInfo stub) =>
        TypeBuilder
            .Parse($"public partial record {stub.RecordName} : {stub.DependencyType.CodeName}")
            .IfNot(string.IsNullOrEmpty(stub.DependencyType.Namespace), b => b.AddUsing(stub.DependencyType.Namespace!))
            .WithPrimaryConstructor(ctor => ctor
                .WithEach(stub.Methods, (c, method) => c
                    .AddParameter($"on{method.Name}", method.DelegateType.Nullable(), p => p.WithDefaultValue("null")))
                .WithXmlDoc(xml => xml
                    .WithSummary($"Primary constructor for {stub.RecordName}.")
                    .WithEach(stub.Methods, (b, method) => b.AddParam($"on{method.Name}", $"Delegate invoked when {method.Name} is called."))))
            .WithEach(stub.Methods, (builder, method) => builder
                .AddMethod(method.CreateStubMethod(stub))
                .AddMethod(method.CreateStubConfigurationMethod(stub)))
            .WithXmlDoc($"Configurable stub implementation of <c>{stub.DependencyType.Name}</c> for testing. Each method delegates to an optional <c>Func</c> property.");

    private static MethodBuilder CreateStubConfigurationMethod(this CapabilityMethodModel method, TestRuntimeWriter.StubInfo stub) =>
        MethodBuilder
            .For($"On{method.Name}")
            .WithReturnType(stub.RecordName)
            .AddParameter("configure", method.DelegateType)
            .WithExpressionBody($"this with {{ on{method.Name} = configure }}")
            .WithXmlDoc(xml => xml
                .WithSummary($"Returns a new {stub.RecordName} with the specified behavior configured for the {method.Name} method.")
                .AddParam("configure", method.DelegateType)
                .WithReturns($"A new {stub.RecordName} instance with the configured behavior."));

    private static MethodBuilder CreateStubMethod(this CapabilityMethodModel method, TestRuntimeWriter.StubInfo stub) =>
        MethodBuilder
            .For(method.Name)
            .ExplicitlyImplements(stub.DependencyType.GloballyQualifiedName)
            .WithReturnType(method.ReturnType)
            .WithEach(method.Parameters, (m, param) => m.AddParameter(param.Name, param.Type))
            .WithExpressionBody(
                TypeRef.From($"on{method.Name}")
                    .Invoke([..method.Parameters.Select(x => x.Name)])
                    .OrDefault(method switch
                    {
                        { IsNonGenericTask: true } => TaskExpression.CompletedTask,
                        { IsNonGenericValueTask: true } => ExpressionRef.From("default"),
                        { IsGenericTask: true } => TaskExpression.FromResult(method.InnerTaskType!, "default!"),
                        { IsGenericValueTask: true } => TaskExpression.FromValueTaskResult("default!"),
                        _ => "default!"
                    }))
            .WithXmlDoc(xml => xml
                .WithSummary($"Stub implementation that delegates to <see cref=\"on{method.Name}\"/> if configured, or returns a default value."));
}