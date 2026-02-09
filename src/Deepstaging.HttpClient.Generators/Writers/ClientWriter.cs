// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;

namespace Deepstaging.HttpClient.Generators.Writers;

/// <summary>
/// Writer for generating the HTTP client class.
/// </summary>
public static class ClientWriter
{
    extension(HttpClientModel model)
    {
        /// <summary>
        /// Generates the HTTP client partial class implementation.
        /// </summary>d
        public OptionalEmit WriteClient()
        {
            return TypeBuilder
                .Parse($"{model.Accessibility} partial class {model.TypeName}")
                .AddUsings("System.Text.Json", "System.Threading", "System.Threading.Tasks", "System.Net.Http")
                .InNamespace(model.Namespace)
                .Implements(model.InterfaceName)
                .WithPrimaryConstructor(c => c
                    .AddParameter("client", "HttpClient")
                    .If(model.HasConfiguration, cb => cb
                        .AddParameter("configuration", model.ConfigurationType!)))
                .AddMethod(MethodBuilder.Parse(
                        """
                        protected async Task<TResponse> SendAsync<TResponse>(
                            HttpRequestMessage request, 
                            CancellationToken token = default
                        )
                        """)
                    .WithBody(b => b
                        .AddStatement("var response = await client.SendAsync(request, token).ConfigureAwait(false);")
                        .AddStatement("response.EnsureSuccessStatusCode();")
                        .AddStatement("var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);")
                        .AddStatement("return JsonSerializer.Deserialize<TResponse>(content)!;")))
                .WithEach(model.Requests, (b, request) => b
                    .AddMethod(request.WriteAsyncMethod()))
                .Emit();
        }
    }
}