using System.Composition;
using Deepstaging.Analyzers;
using Deepstaging.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.CodeFixes;

/// <summary>
/// Code fix provider that adds the [Runtime] attribute to classes with [Uses].
/// </summary>
[Shared]
[CodeFix(UsesAttributeMustTargetRuntimeAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UsesAttributeMustTargetRuntimeCodeFix))]
public sealed class UsesAttributeMustTargetRuntimeCodeFix : ClassCodeFix
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax) =>
        document.AddAttributeAction(syntax, "Deepstaging.Runtime");
}
