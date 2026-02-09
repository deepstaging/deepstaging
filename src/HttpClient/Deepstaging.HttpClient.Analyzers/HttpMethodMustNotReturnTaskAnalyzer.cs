// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.HttpClient.Analyzers;

/// <summary>
/// Reports a diagnostic when a method with an HTTP verb attribute returns Task.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "HTTP method must not return Task",
    Message = "Method '{0}' should return the response type directly, not wrapped in Task. The generator creates the async version.",
    Description =
        "Methods decorated with HTTP verb attributes should return the response type directly (e.g., 'User' not 'Task<User>'). The source generator creates the async implementation.")]
public sealed class HttpMethodMustNotReturnTaskAnalyzer : MethodAnalyzer
{
    /// <summary>
    /// Diagnostic ID for returning Task from HTTP method definition.
    /// </summary>
    public const string DiagnosticId = "HTTP003";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<IMethodSymbol> method) =>
        HasHttpVerbAttribute(method) && ReturnsTask(method);

    private static bool HasHttpVerbAttribute(ValidSymbol<IMethodSymbol> method) =>
        method.HasAttribute<GetAttribute>() ||
        method.HasAttribute<PostAttribute>() ||
        method.HasAttribute<PutAttribute>() ||
        method.HasAttribute<PatchAttribute>() ||
        method.HasAttribute<DeleteAttribute>();

    private static bool ReturnsTask(ValidSymbol<IMethodSymbol> method)
    {
        var returnType = method.Value.ReturnType;
        var typeName = returnType.ToDisplayString();
        return typeName.StartsWith("System.Threading.Tasks.Task") ||
               typeName.StartsWith("global::System.Threading.Tasks.Task");
    }
}
