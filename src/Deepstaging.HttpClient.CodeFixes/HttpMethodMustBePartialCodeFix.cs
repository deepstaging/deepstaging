// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Composition;
using Deepstaging.HttpClient.Analyzers;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.HttpClient.CodeFixes;

/// <summary>
/// Code fix provider that adds the 'partial' modifier to methods with HTTP verb attributes.
/// </summary>
[Shared]
[CodeFix(HttpMethodMustBePartialAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpMethodMustBePartialCodeFix))]
public sealed class HttpMethodMustBePartialCodeFix : MethodCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}