// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.HttpClient;

using Deepstaging.Projection.HttpClient.Models;
using Roslyn.Expressions;

/// <summary>
/// Writer for generating the HTTP client class.
/// </summary>
public static class ClientWriter
{
    extension(HttpClientModel model)
    {
        /// <summary>
        /// Generates the HTTP client partial class implementation.
        /// </summary>
        public OptionalEmit WriteClient() => TypeBuilder
            .Parse($"{model.Accessibility} partial class {model.TypeName}")
            .AddUsings(HttpTypes.Namespace, TaskTypes.Namespace, TaskTypes.Namespace, JsonTypes.Namespace)
            .InNamespace(model.Namespace)
            .Implements(model.InterfaceName)
            .WithPrimaryConstructor(c => c
                .AddParameter("client", HttpTypes.Client)
                .If(model.HasConfiguration, cb => cb
                    .AddParameter("configuration", model.ConfigurationType!)))
            .AddMethod(SendAsyncMethod)
            .WithEach(model.Requests, (b, request) => b
                .AddMethod(request.WriteAsyncMethod()))
            .Emit();
    }

    private static MethodBuilder SendAsyncMethod => MethodBuilder
        .Parse($"""
                protected async {TaskTypes.Task("TResponse")} SendAsync<TResponse>(
                    {HttpTypes.RequestMessage} request, 
                    {TaskTypes.CancellationToken} token = default
                )
                """)
        .WithBody(b => b
            .AddStatement($"var response = await {HttpExpression.SendAsync("client", "request", "token").ConfigureAwait()}")
            .AddStatement(HttpExpression.EnsureSuccessStatusCode("response"))
            .AddStatement($"var content = await {HttpExpression.ReadAsStringAsync("response").ConfigureAwait()}")
            .AddReturn(JsonExpression.Deserialize("TResponse", "content").NullForgiving())
        );
}