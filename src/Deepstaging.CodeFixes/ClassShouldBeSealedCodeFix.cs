// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes;

using Analyzers.Effects;

/// <summary>
/// Code fix provider that adds the 'sealed' modifier to classes with [EffectsModule].
/// </summary>
[Shared]
[CodeFix(EffectsModuleShouldBeSealedAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassShouldBeSealedCodeFix))]
public sealed class ClassShouldBeSealedCodeFix : ClassCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddSealedModifierAction(syntax);
}