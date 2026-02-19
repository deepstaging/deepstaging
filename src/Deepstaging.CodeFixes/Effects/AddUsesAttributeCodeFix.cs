// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes.Effects;

using System.Collections.Immutable;
using Analyzers.Effects;

/// <summary>
/// Code fix provider that adds a <c>[Uses(typeof(X))]</c> attribute to a <c>[Runtime]</c> class
/// for an available but unreferenced effects module.
/// </summary>
/// <remarks>
/// This uses a raw <see cref="CodeFixProvider"/> instead of <see cref="ClassCodeFix"/> because
/// it needs access to <see cref="Diagnostic.Properties"/> to read the module type name,
/// which is not available through the <see cref="SyntaxCodeFix{TSyntax}.CreateFix"/> signature.
/// </remarks>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddUsesAttributeCodeFix))]
public sealed class AddUsesAttributeCodeFix : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds => [AvailableEffectsModuleAnalyzer.DiagnosticId];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];

        if (!diagnostic.Properties.TryGetValue(AvailableEffectsModuleAnalyzer.ModuleTypeProperty, out var moduleTypeName)
            || string.IsNullOrEmpty(moduleTypeName))
            return;

        var result = await context.FindDeclaration<ClassDeclarationSyntax>().ConfigureAwait(false);

        if (result.IsNotValid(out var syntax))
            return;

        var codeAction = context.Document.AddAttributeAction(syntax, "Uses", $"typeof({moduleTypeName})");
        context.RegisterCodeFix(codeAction, diagnostic);
    }
}
