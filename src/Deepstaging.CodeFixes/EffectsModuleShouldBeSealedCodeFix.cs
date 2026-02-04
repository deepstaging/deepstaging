using System.Composition;
using Deepstaging.Analyzers;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.CodeFixes;

/// <summary>
/// Code fix provider that adds the 'sealed' modifier to classes with [EffectsModule].
/// </summary>
[Shared]
[CodeFix(EffectsModuleShouldBeSealedAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EffectsModuleShouldBeSealedCodeFix))]
public sealed class EffectsModuleShouldBeSealedCodeFix : ClassCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddSealedModifierAction(syntax);
}
