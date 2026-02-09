// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Microsoft.CodeAnalysis;

namespace Deepstaging.HttpClient.Generators.Writers;

/// <summary>
/// Writer for generating HTTP request records.
/// </summary>
public static class RequestWriter
{
    extension(HttpRequestModel request)
    {
        /// <summary>
        /// Generates the request record type.
        /// </summary>
        public OptionalEmit WriteRequest(HttpClientModel client)
        {
            return TypeBuilder
                .Parse($"public sealed record {request.RequestTypeName}")
                .InNamespace(client.Namespace)
                .AddProperty("Path", "string", p => p.AsRequired())
                .Emit();
        }

        /// <summary>
        /// Generates the async method implementation for this request.
        /// </summary>
        public MethodBuilder WriteAsyncMethod()
        {
            var returnType = $"global::System.Threading.Tasks.Task<{request.ReturnType}>";

            var method = MethodBuilder
                .For(request.AsyncMethodName)
                .WithAccessibility(Accessibility.Public)
                .Async()
                .WithReturnType(returnType);

            // Add parameters
            foreach (var param in request.Parameters)
            {
                method = param.HasDefaultValue
                    ? method.AddParameter(param.Name, param.TypeFqn, p => p.WithDefaultValue(param.DefaultValueExpression!))
                    : method.AddParameter(param.Name, param.TypeFqn);
            }

            // Add cancellation token
            method = method.AddParameter("token", "global::System.Threading.CancellationToken", p => p.WithDefaultValue("default"));

            // Build method body
            method = method.WithBody(b =>
            {
                // Build the path with interpolation
                var pathExpr = BuildPathExpression(request);

                // Build query string if needed
                if (request.QueryParameters.Length > 0)
                {
                    b = b.AddStatement("var queryParams = new global::System.Collections.Generic.List<string>();");
                    foreach (var qp in request.QueryParameters)
                    {
                        var queryName = qp.EffectiveName;
                        b = b.AddStatement($"if ({qp.Name} != null) queryParams.Add($\"{queryName}={{{qp.Name}}}\");");
                    }
                    b = b.AddStatement($"var path = queryParams.Count > 0 ? $\"{pathExpr}?{{string.Join(\"&\", queryParams)}}\" : $\"{pathExpr}\";");
                }
                else
                {
                    b = b.AddStatement($"var path = $\"{pathExpr}\";");
                }

                // Create HTTP request message
                b = b.AddStatement($"using var httpRequest = new global::System.Net.Http.HttpRequestMessage(global::System.Net.Http.HttpMethod.{request.Verb}, path);");

                // Add headers
                foreach (var hp in request.HeaderParameters)
                {
                    b = b.AddStatement($"if ({hp.Name} != null) httpRequest.Headers.TryAddWithoutValidation(\"{hp.EffectiveName}\", {hp.Name});");
                }

                // Add body if present
                if (request.BodyParameter is { } body)
                {
                    b = b.AddStatement($"var json = global::System.Text.Json.JsonSerializer.Serialize({body.Name});");
                    b = b.AddStatement("httpRequest.Content = new global::System.Net.Http.StringContent(json, global::System.Text.Encoding.UTF8, \"application/json\");");
                }

                // Send request
                b = b.AddStatement($"return await SendAsync<{request.ReturnType}>(httpRequest, token).ConfigureAwait(false);");

                return b;
            });

            return method;
        }

        private static string BuildPathExpression(HttpRequestModel model)
        {
            // Replace {param} with {param} for string interpolation
            var path = model.Path;
            foreach (var param in model.PathParameters)
            {
                path = path.Replace($"{{{param.Name}}}", $"{{{param.Name}}}");
            }
            return path;
        }
    }
}
