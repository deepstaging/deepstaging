// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Composition;
using Deepstaging.Effects.Analyzers;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Effects.CodeFixes;

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
