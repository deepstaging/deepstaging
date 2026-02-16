// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.HttpClient.Models;

/// <summary>
/// Model representing an HTTP request method.
/// </summary>
[PipelineModel]
public sealed record HttpRequestModel
{
    /// <summary>
    /// The HTTP verb (GET, POST, etc.).
    /// </summary>
    public required HttpVerb Verb { get; init; }

    /// <summary>
    /// The method name as defined in the source.
    /// </summary>
    public required string MethodName { get; init; }

    /// <summary>
    /// The request path template (e.g., "/users/{id}").
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// The fully qualified return type.
    /// </summary>
    public required string ReturnType { get; init; }

    /// <summary>
    /// Whether the return type is a value type.
    /// </summary>
    public required bool ReturnsValueType { get; init; }

    /// <summary>
    /// The method parameters.
    /// </summary>
    public required EquatableArray<HttpParameterModel> Parameters { get; init; }

    /// <summary>
    /// The async method name (MethodName + "Async").
    /// </summary>
    public string AsyncMethodName => $"{MethodName}Async";

    /// <summary>
    /// The request record type name (MethodName + "Request").
    /// </summary>
    public string RequestTypeName => $"{MethodName}Request";

    /// <summary>
    /// Parameters that are path parameters.
    /// </summary>
    public EquatableArray<HttpParameterModel> PathParameters =>
        [.. Parameters.Where(p => p.Kind == ParameterKind.Path)];

    /// <summary>
    /// Parameters that are query parameters.
    /// </summary>
    public EquatableArray<HttpParameterModel> QueryParameters =>
        [.. Parameters.Where(p => p.Kind == ParameterKind.Query)];

    /// <summary>
    /// Parameters that are header parameters.
    /// </summary>
    public EquatableArray<HttpParameterModel> HeaderParameters =>
        [.. Parameters.Where(p => p.Kind == ParameterKind.Header)];

    /// <summary>
    /// The body parameter, if any.
    /// </summary>
    public HttpParameterModel? BodyParameter =>
        Parameters.FirstOrDefault(p => p.Kind == ParameterKind.Body);
}