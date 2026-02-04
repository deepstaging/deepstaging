// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Composition;
using Deepstaging.Analyzers;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.CodeFixes;

/// <summary>
/// Code fix provider that adds the 'partial' modifier to classes with [EffectsModule].
/// </summary>
[Shared]
[CodeFix(EffectsModuleMustBePartialAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EffectsModuleMustBePartialCodeFix))]
public sealed class EffectsModuleMustBePartialCodeFix : ClassCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}
