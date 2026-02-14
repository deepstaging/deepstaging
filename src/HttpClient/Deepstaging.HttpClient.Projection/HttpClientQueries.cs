// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Deepstaging.HttpClient.Projection.Attributes;
using Deepstaging.HttpClient.Projection.Models;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;

namespace Deepstaging.HttpClient.Projection;

/// <summary>
/// Extension methods for querying HTTP client attributes from symbols.
/// </summary>
public static class HttpClientQueries
{
    private static readonly Regex PathParameterRegex = new(@"\{(\w+)\}", RegexOptions.Compiled);

    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Creates an <see cref="HttpClientModel"/> from the symbol.
        /// </summary>
        public HttpClientModel? QueryHttpClient()
        {
            // Check for both generic and non-generic HttpClientAttribute
            return symbol.GetAttribute<HttpClientAttribute>()
                .OrElse(() => symbol.GetAttribute(typeof(HttpClientAttribute<>)))
                .Map(attr => attr.AsQuery<HttpClientAttributeQuery>())
                .Map(query => new HttpClientModel
                {
                    Namespace = symbol.Namespace ?? "",
                    TypeName = symbol.Name,
                    Accessibility = symbol.AccessibilityString,
                    ConfigurationType = query.ConfigurationType.FullyQualifiedName,
                    BaseAddress = query.BaseAddress,
                    Requests =
                    [
                        ..symbol.QueryMethods()
                            .ThatArePartialDefinitions()
                            .Select(method => method.QueryHttpRequest())
                            .Where(model => model.HasValue)
                            .Select(model => model.Value)
                    ]
                })
                .OrNull();
        }
    }

    extension(ValidSymbol<IMethodSymbol> method)
    {
        /// <summary>
        /// Creates an <see cref="HttpRequestModel"/> from the method if it has an HTTP verb attribute.
        /// </summary>
        private OptionalValue<HttpRequestModel> QueryHttpRequest()
        {
            return method.GetAttribute<GetAttribute>()
                .OrElse(() => method.GetAttribute<PostAttribute>())
                .OrElse(() => method.GetAttribute<PutAttribute>())
                .OrElse(() => method.GetAttribute<PatchAttribute>())
                .OrElse(() => method.GetAttribute<DeleteAttribute>())
                .Map(attr => attr.AsQuery<HttpVerbAttributeQuery>())
                .Map(attr => new HttpRequestModel
                {
                    Verb = attr.Verb,
                    MethodName = method.Name,
                    Path = attr.Path,
                    ReturnType = method.ReturnType.FullyQualifiedName,
                    ReturnsValueType = method.ReturnType.IsValueType,
                    Parameters = method.QueryHttpParameters(attr.Path)
                });
        }

        /// <summary>
        /// Queries all parameters of this method and infers their binding.
        /// </summary>
        public ImmutableArray<HttpParameterModel> QueryHttpParameters(string path)
        {
            var pathParams = PathParameterRegex
                .Matches(path)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToImmutableHashSet();

            return
            [
                ..method.Parameters.Select(param => param.QueryHttpParameter(pathParams))
            ];
        }
    }

    extension(ValidSymbol<IParameterSymbol> param)
    {
        /// <summary>
        /// Creates an <see cref="HttpParameterModel"/> from the parameter.
        /// </summary>
        public HttpParameterModel QueryHttpParameter(ImmutableHashSet<string> pathParams)
        {
            var (kind, customName) = param.GetParameterKind(pathParams);

            return new HttpParameterModel
            {
                Name = param.Name,
                TypeFqn = param.Type.FullyQualifiedName,
                Kind = kind,
                CustomName = customName,
                HasDefaultValue = param.HasExplicitDefaultValue,
                DefaultValueExpression = param.ExplicitDefaultValue.Map(o => o switch
                {
                    null => "null",
                    string s => $"\"{s}\"",
                    bool b => b ? "true" : "false",
                    var v => v.ToString()
                }).OrNull()
            };
        }

        private (ParameterKind, string?) GetParameterKind(ImmutableHashSet<string> pathParams)
        {
            var fromAttribute = param.GetAttributes()
                .Select(attr => attr.AttributeClass?.Name switch
                {
                    "BodyAttribute" => (ParameterKind.Body, null),
                    "QueryAttribute" => (ParameterKind.Query, attr.ConstructorArg<string>(0).OrNull()),
                    "HeaderAttribute" => (ParameterKind.Header, attr.ConstructorArg<string>(0).OrNull()),
                    "PathAttribute" => (ParameterKind.Path, null),
                    _ => ((ParameterKind, string?)?)null
                })
                .FirstOrDefault(x => x.HasValue);
            return fromAttribute
                   ?? (pathParams.Contains(param.Name) ? (ParameterKind.Path, null) : (ParameterKind.Query, null));
        }
    }
}