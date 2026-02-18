// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.HttpClient;

/// <summary>
/// Reports a diagnostic when a method with an HTTP verb attribute returns Task.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "HTTP method must not return Task",
    Message = "Method '{0}' should return the response type directly, not wrapped in Task. The generator creates the async version.",
    Description =
        "Methods decorated with HTTP verb attributes should return the response type directly (e.g., 'User' not 'Task<User>'). The source generator creates the async implementation."
)]
public sealed class HttpMethodMustNotReturnTaskAnalyzer : MethodAnalyzer
{
    /// <summary>
    /// Diagnostic ID for returning Task from HTTP method definition.
    /// </summary>
    public const string DiagnosticId = "DSHTTP03";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<IMethodSymbol> method) =>
        HasHttpVerbAttribute(method) && method.ReturnType.IsTask;

    private static bool HasHttpVerbAttribute(ValidSymbol<IMethodSymbol> method) =>
        method.HasAttribute<GetAttribute>() ||
        method.HasAttribute<PostAttribute>() ||
        method.HasAttribute<PutAttribute>() ||
        method.HasAttribute<PatchAttribute>() ||
        method.HasAttribute<DeleteAttribute>();
}