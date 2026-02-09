// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.HttpClient.Analyzers;

/// <summary>
/// Reports a diagnostic when a class with [HttpClient] is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "HttpClient class must be partial",
    Message = "Class '{0}' has [HttpClient] attribute but is not declared as partial",
    Description =
        "Classes decorated with [HttpClient] must be declared as partial because the source generator emits additional partial class members.")]
public sealed class HttpClientMustBePartialAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing partial modifier on HttpClient class.
    /// </summary>
    public const string DiagnosticId = "HTTP001";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        HasHttpClientAttribute(type) && type is { IsPartial: false };

    private static bool HasHttpClientAttribute(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<HttpClientAttribute>() ||
        type.HasAttribute("Deepstaging.HttpClient.HttpClientAttribute`1");
}
