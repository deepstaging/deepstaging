// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.HttpClient;

using Projection.HttpClient.Models;

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
        public OptionalEmit WriteRequest(HttpClientModel client) =>
            TypeBuilder
                .Parse($"public sealed record {request.RequestTypeName}")
                .InNamespace(client.Namespace)
                .AddProperty("Path", "string", p => p.AsRequired())
                .Emit();

        /// <summary>
        /// Generates the async method implementation for this request.
        /// </summary>
        public MethodBuilder WriteAsyncMethod() =>
            MethodBuilder
                .Parse($"public async {Tasks.Task(request.ReturnType)} {request.AsyncMethodName}()")
                .WithEach(request.Parameters, (b, param) => param.HasDefaultValue
                    ? b.AddParameter(param.Name, param.TypeFqn, p => p.WithDefaultValue(param.DefaultValueExpression!))
                    : b.AddParameter(param.Name, param.TypeFqn))
                .AddParameter("token", Tasks.CancellationToken, p => p.WithDefaultValue("default"))
                .WithBody(body => body
                    .AddPathStatements(request)
                    .AddStatement($"using var httpRequest = new {Http.RequestMessage}({Http.Verb($"{request.Verb}")}, path);")
                    .WithEach(request.HeaderParameters, (b, hp) => b
                        .AddStatement($"if ({hp.Name} != null) httpRequest.Headers.TryAddWithoutValidation(\"{hp.EffectiveName}\", {hp.Name});"))
                    .If(condition: request.BodyParameter is not null, b => b
                        .AddStatement($"var json = {Json.Serializer}.Serialize({request.BodyParameter!.Name})")
                        .AddStatement($"httpRequest.Content = new {Http.StringContent}(json, {Encoding.UTF8}, \"application/json\");"))
                    .AddReturn($"await SendAsync<{request.ReturnType}>(httpRequest, token).ConfigureAwait(false);"));
    }

    private static BodyBuilder AddPathStatements(this BodyBuilder builder, HttpRequestModel request)
    {
        var pathExpr = request.PathParameters.Aggregate(request.Path, (current, param) =>
            current.Replace(oldValue: $"{{{param.Name}}}", newValue: $"{{{param.Name}}}"));

        return builder.If(
            condition: request.QueryParameters.Count > 0,
            then: body => body
                .AddStatement($"var params = new {Collections.List("string")}()")
                .WithEach(request.QueryParameters, (b, qp) => b
                    .AddStatement($"if ({qp.Name} != null) params.Add($\"{qp.EffectiveName}={{{qp.Name}}}\");"))
                .AddStatement($"var path = params.Count > 0 ? $\"{pathExpr}?{{string.Join(\"&\", params)}}\" : $\"{pathExpr}\";"),
            @else: body => body
                .AddStatement($"var path = $\"{pathExpr}\";")
        );
    }
}