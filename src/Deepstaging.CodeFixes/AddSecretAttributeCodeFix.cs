// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes;

using Analyzers.Config;

/// <summary>
/// Code fix provider that adds the <c>[Secret]</c> attribute to properties flagged by CFG005.
/// </summary>
[Shared]
[CodeFix(PotentialSecretPropertyAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddSecretAttributeCodeFix))]
public sealed class AddSecretAttributeCodeFix : PropertyCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<PropertyDeclarationSyntax> syntax) =>
        document.AddAttributeAction(syntax, "Deepstaging.Config.Secret");
}
