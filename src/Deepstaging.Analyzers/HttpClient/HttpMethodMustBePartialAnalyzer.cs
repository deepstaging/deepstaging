// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.HttpClient;

/// <summary>
/// Reports a diagnostic when a method with an HTTP verb attribute is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "HTTP method must be partial",
    Message = "Method '{0}' has an HTTP verb attribute but is not declared as partial",
    Description =
        "Methods decorated with HTTP verb attributes ([Get], [Post], etc.) must be declared as partial because the source generator provides the implementation."
)]
public sealed class HttpMethodMustBePartialAnalyzer : MethodAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing partial modifier on HTTP method.
    /// </summary>
    public const string DiagnosticId = "DSHTTP02";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<IMethodSymbol> method) =>
        HasHttpVerbAttribute(method) && !method.IsPartialMethod();

    private static bool HasHttpVerbAttribute(ValidSymbol<IMethodSymbol> method) =>
        method.HasAttribute<GetAttribute>() ||
        method.HasAttribute<PostAttribute>() ||
        method.HasAttribute<PutAttribute>() ||
        method.HasAttribute<PatchAttribute>() ||
        method.HasAttribute<DeleteAttribute>();
}