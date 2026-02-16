// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.HttpClient;

/// <summary>
/// Reports a diagnostic when a method with an HTTP verb attribute has an empty path.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "HTTP path must not be empty",
    Message = "Method '{0}' has an HTTP verb attribute with an empty or whitespace path",
    Description =
        "Methods decorated with HTTP verb attributes must specify a non-empty path in the attribute constructor."
)]
public sealed class HttpPathMustNotBeEmptyAnalyzer : MethodAnalyzer
{
    /// <summary>
    /// Diagnostic ID for empty HTTP path.
    /// </summary>
    public const string DiagnosticId = "HTTP004";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<IMethodSymbol> method)
    {
        var path = GetHttpPath(method);
        return path is not null && string.IsNullOrWhiteSpace(path);
    }

    private static string? GetHttpPath(ValidSymbol<IMethodSymbol> method) =>
        method.GetAttributes()
            .Where(attr =>
                attr.Is<GetAttribute>() ||
                attr.Is<PostAttribute>() ||
                attr.Is<PutAttribute>() ||
                attr.Is<PatchAttribute>() ||
                attr.Is<DeleteAttribute>()
            )
            .Select(attr => attr.ConstructorArg<string>(0).OrNull())
            .FirstOrDefault();
}