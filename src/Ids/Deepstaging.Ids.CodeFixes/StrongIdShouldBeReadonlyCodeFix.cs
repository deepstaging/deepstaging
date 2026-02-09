// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Composition;
using Deepstaging.Ids.Analyzers;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Ids.CodeFixes;

/// <summary>
/// Code fix provider that adds the 'readonly' modifier to structs with [StrongId].
/// </summary>
[Shared]
[CodeFix(StrongIdShouldBeReadonlyAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StrongIdShouldBeReadonlyCodeFix))]
public sealed class StrongIdShouldBeReadonlyCodeFix : StructCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddModifierAction(syntax, SyntaxKind.ReadOnlyKeyword, "Add 'readonly' modifier");
}
