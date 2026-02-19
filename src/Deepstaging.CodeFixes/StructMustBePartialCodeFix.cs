// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes;

/// <summary>
/// Code fix provider that adds the 'partial' modifier to structs.
/// </summary>
[Shared]
[CodeFix(TypedIdMustBePartialAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StructMustBePartialCodeFix))]
public sealed class StructMustBePartialCodeFix : StructCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}