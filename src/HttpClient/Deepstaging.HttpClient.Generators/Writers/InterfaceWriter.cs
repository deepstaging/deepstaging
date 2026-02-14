// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;

namespace Deepstaging.HttpClient.Generators.Writers;

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
        public OptionalEmit WriteInterface()
        {
            var builder = TypeBuilder
                .Parse($"{model.Accessibility} partial interface {model.InterfaceName}")
                .InNamespace(model.Namespace);

            // Add interface methods for each request
            foreach (var request in model.Requests)
            {
                builder = builder.AddMethod(request.WriteInterfaceMethod());
            }

            return builder.Emit();
        }
    }

    extension(HttpRequestModel request)
    {
        /// <summary>
        /// Generates an interface method signature for this request.
        /// </summary>
        public MethodBuilder WriteInterfaceMethod()
        {
            var returnType = $"global::System.Threading.Tasks.Task<{request.ReturnType}>";

            var method = MethodBuilder
                .For(request.AsyncMethodName)
                .WithReturnType(returnType)
                .AsAbstract(); // Interface methods have no body

            // Add parameters
            foreach (var param in request.Parameters)
            {
                method = param.HasDefaultValue
                    ? method.AddParameter(param.Name, param.TypeFqn, p => p.WithDefaultValue(param.DefaultValueExpression!))
                    : method.AddParameter(param.Name, param.TypeFqn);
            }

            // Add cancellation token
            method = method.AddParameter("token", "global::System.Threading.CancellationToken", p => p.WithDefaultValue("default"));

            return method;
        }
    }
}
