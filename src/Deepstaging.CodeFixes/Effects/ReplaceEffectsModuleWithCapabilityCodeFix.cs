// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes.Effects;

using Analyzers.Effects;

/// <summary>
/// Code fix provider that replaces [EffectsModule(typeof(X))] with [Capability(typeof(X))]
/// when the target type has no methods to lift into effects.
/// </summary>
[Shared]
[CodeFix(EffectsModuleTargetHasNoMethodsAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReplaceEffectsModuleWithCapabilityCodeFix))]
public sealed class ReplaceEffectsModuleWithCapabilityCodeFix : ClassCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.ReplaceAttributeAction(syntax, "EffectsModule", "Capability");
}
