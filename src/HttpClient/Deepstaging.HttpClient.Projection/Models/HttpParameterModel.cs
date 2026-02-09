// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.HttpClient.Projection.Models;

/// <summary>
/// Model representing a parameter of an HTTP request method.
/// </summary>
public sealed record HttpParameterModel
{
    /// <summary>
    /// The parameter name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The fully qualified type name.
    /// </summary>
    public required string TypeFqn { get; init; }

    /// <summary>
    /// How this parameter is bound in the request.
    /// </summary>
    public required ParameterKind Kind { get; init; }

    /// <summary>
    /// For Header parameters, the HTTP header name.
    /// For Query parameters, the query parameter name (if different from Name).
    /// </summary>
    public required string? CustomName { get; init; }

    /// <summary>
    /// Whether this parameter has a default value.
    /// </summary>
    public required bool HasDefaultValue { get; init; }

    /// <summary>
    /// The default value expression as a string, if HasDefaultValue is true.
    /// </summary>
    public required string? DefaultValueExpression { get; init; }

    /// <summary>
    /// The effective name used in HTTP (CustomName ?? Name).
    /// </summary>
    public string EffectiveName => CustomName ?? Name;
}
