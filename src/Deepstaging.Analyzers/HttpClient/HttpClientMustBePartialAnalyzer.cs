// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.HttpClient;

/// <summary>
/// Reports a diagnostic when a class with [HttpClient] is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "HttpClient class must be partial",
    Message = "Class '{0}' has [HttpClient] attribute but is not declared as partial",
    Description =
        "Classes decorated with [HttpClient] must be declared as partial because the source generator emits additional partial class members."
)]
public sealed class HttpClientMustBePartialAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing partial modifier on HttpClient class.
    /// </summary>
    public const string DiagnosticId = "DSHTTP01";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        HasHttpClientAttribute(type) && type is { IsPartial: false };

    private static bool HasHttpClientAttribute(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<HttpClientAttribute>() || type.HasAttribute(typeof(HttpClientAttribute<>));
}