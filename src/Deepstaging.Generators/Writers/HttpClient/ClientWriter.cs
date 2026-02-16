// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.HttpClient;

using Deepstaging.Projection.HttpClient.Models;

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
            .AddUsings(HttpRefs.Namespace, TaskRefs.ThreadingNamespace, TaskRefs.Namespace, JsonRefs.Namespace)
            .InNamespace(model.Namespace)
            .Implements(model.InterfaceName)
            .WithPrimaryConstructor(c => c
                .AddParameter("client", HttpRefs.Client)
                .If(model.HasConfiguration, cb => cb
                    .AddParameter("configuration", model.ConfigurationType!)))
            .AddMethod(SendAsyncMethod)
            .WithEach(model.Requests, (b, request) => b
                .AddMethod(request.WriteAsyncMethod()))
            .Emit();
    }

    private static MethodBuilder SendAsyncMethod => MethodBuilder
        .Parse($"""
                protected async {TaskRefs.Task("TResponse")} SendAsync<TResponse>(
                    {HttpRefs.RequestMessage} request, 
                    {TaskRefs.CancellationToken} token = default
                )
                """)
        .WithBody(b => b
            .AddStatement($"var response = await {HttpRefs.SendAsync("client", "request", "token").ConfigureAwait()}")
            .AddStatement(HttpRefs.EnsureSuccessStatusCode("response"))
            .AddStatement($"var content = await {HttpRefs.ReadAsStringAsync("response").ConfigureAwait()}")
            .AddReturn(JsonRefs.Deserialize("TResponse", "content").NullForgiving())
        );
}