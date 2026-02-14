// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Composition;
using Deepstaging.Effects.Testing.Analyzers;
using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Effects.Testing.CodeFixes;

/// <summary>
/// Code fix provider that scaffolds <c>WithXxxSuccess()</c> and <c>WithXxxError()</c>
/// scenario helper methods on a <c>[TestRuntime&lt;T&gt;]</c> class for a given capability.
/// </summary>
[Shared]
[CodeFix(TestRuntimeCapabilityInfoAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(GenerateCapabilityScenarioCodeFix))]
public sealed class GenerateCapabilityScenarioCodeFix : ClassCodeFix
{
    /// <inheritdoc />
    protected override CodeAction? CreateFix(Document document, ValidSyntax<ClassDeclarationSyntax> syntax)
    {
        // TODO: Implement using Emit API + AddMembersFromSourceAction
        //
        // 1. Resolve which capability this diagnostic is for (from diagnostic properties or re-query)
        // 2. Build success/error scenario methods via Emit:
        //
        //   public TestAppRuntime WithEmailServiceSuccess(List<Email> inbox) =>
        //       WithEmailService(new StubEmailService
        //       {
        //           OnGetInbox = () => Task.FromResult(inbox),
        //           OnSendEmail = (_, _) => Task.CompletedTask
        //       });
        //
        //   public TestAppRuntime WithEmailServiceError(Exception ex) =>
        //       WithEmailService(new StubEmailService
        //       {
        //           OnGetInbox = () => throw ex,
        //           OnSendEmail = (_, _) => throw ex
        //       });
        //
        // 3. Return: document.AddMembersFromSourceAction(syntax, title, emittedSource);
        
        

        return null;
    }
}
