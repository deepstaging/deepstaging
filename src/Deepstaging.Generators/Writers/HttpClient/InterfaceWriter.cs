// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.HttpClient;

using Projection.HttpClient.Models;

/// <summary>
/// Writer for generating the HTTP client interface.
/// </summary>
public static class InterfaceWriter
{
    extension(HttpClientModel model)
    {
        /// <summary>
        /// Generates the HTTP client interface.
        /// </summary>
        public OptionalEmit WriteInterface() => TypeBuilder
            .Parse($"{model.Accessibility} partial interface {model.InterfaceName}")
            .InNamespace(model.Namespace)
            .WithEach(model.Requests, (b, request) => b
                .AddMethod(request.WriteInterfaceMethod()))
            .Emit();
    }

    /// <summary>
    /// Generates an interface method signature for this request.
    /// </summary>
    private static MethodBuilder WriteInterfaceMethod(this HttpRequestModel request) =>
        MethodBuilder
            .For(request.AsyncMethodName)
            .WithReturnType(Tasks.Task(request.ReturnType))
            .AsAbstract()
            .WithEach(request.Parameters, (b, param) => param.HasDefaultValue
                ? b.AddParameter(param.Name, param.TypeFqn, p => p.WithDefaultValue(param.DefaultValueExpression!))
                : b.AddParameter(param.Name, param.TypeFqn))
            .AddParameter("token", Tasks.CancellationToken, p => p.WithDefaultValue("default"));
}