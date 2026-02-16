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
            .AddUsings("System.Text.Json", "System.Threading", "System.Threading.Tasks", "System.Net.Http")
            .InNamespace(model.Namespace)
            .Implements(model.InterfaceName)
            .WithPrimaryConstructor(c => c
                .AddParameter("client", Http.Client)
                .If(model.HasConfiguration, cb => cb
                    .AddParameter("configuration", model.ConfigurationType!)))
            .AddMethod(SendAsyncMethod)
            .WithEach(model.Requests, (b, request) => b
                .AddMethod(request.WriteAsyncMethod()))
            .Emit();
    }

    private static MethodBuilder SendAsyncMethod => MethodBuilder
        .Parse($"""
                protected async {Tasks.Task("TResponse")} SendAsync<TResponse>(
                    {Http.RequestMessage} request, 
                    {Tasks.CancellationToken} token = default
                )
                """)
        .WithBody(b => b
            .AddStatement("var response = await client.SendAsync(request, token).ConfigureAwait(false)")
            .AddStatement("response.EnsureSuccessStatusCode()")
            .AddStatement("var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false)")
            .AddReturn($"{Json.Serializer}.Deserialize<TResponse>(content)!;")
        );
}