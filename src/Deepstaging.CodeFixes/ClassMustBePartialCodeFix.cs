// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes;

using Analyzers.Effects;

/// <summary>
/// Code fix provider that adds the 'partial' modifier to classes with [EffectsModule].
/// </summary>
[Shared]
[CodeFix(RuntimeMustBePartialAnalyzer.DiagnosticId)]
[CodeFix(EffectsModuleMustBePartialAnalyzer.DiagnosticId)]
[CodeFix(HttpClientMustBePartialAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassMustBePartialCodeFix))]
public sealed class ClassMustBePartialCodeFix : ClassCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}