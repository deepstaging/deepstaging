// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes;

/// <summary>
/// Code fix provider that adds the 'partial' modifier to methods
/// </summary>
[Shared]
[CodeFix(HttpMethodMustBePartialAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodMustBePartialCodeFix))]
public sealed class MethodMustBePartialCodeFix : MethodCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<MethodDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}