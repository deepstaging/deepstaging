// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes;

/// <summary>
/// Code fix provider that adds the 'readonly' modifier to structs.
/// </summary>
[Shared]
[CodeFix(StrongIdShouldBeReadonlyAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StructShouldBeReadonlyCodeFix))]
public sealed class StructShouldBeReadonlyCodeFix : StructCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddModifierAction(syntax, SyntaxKind.ReadOnlyKeyword, "Add 'readonly' modifier");
}