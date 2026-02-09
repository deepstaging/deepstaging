// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Composition;
using Deepstaging.Ids.Analyzers;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Ids.CodeFixes;

/// <summary>
/// Code fix provider that adds the 'partial' modifier to structs with [StrongId].
/// </summary>
[Shared]
[CodeFix(StrongIdMustBePartialAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StrongIdMustBePartialCodeFix))]
public sealed class StrongIdMustBePartialCodeFix : StructCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}
