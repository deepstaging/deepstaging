// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes.Config;

/// <summary>
/// Code fix provider that adds a <c>&lt;UserSecretsId&gt;</c> to <c>deepstaging.props</c>
/// when a <c>[ConfigProvider]</c> exposes <c>[Secret]</c> properties.
/// </summary>
[Shared]
[CodeFix(MissingUserSecretsIdAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddUserSecretsIdCodeFix))]
public sealed class AddUserSecretsIdCodeFix : ProjectCodeFix
{
    /// <inheritdoc />
    protected override CodeAction? CreateFix(Project project, Diagnostic diagnostic)
    {
        var secretsId = Guid.NewGuid().ToString();

        return project.ModifyPropsFileAction<DeepstagingProps>(
            "Add UserSecretsId to deepstaging.props",
            doc => doc.SetProperty("UserSecretsId", secretsId,
                comment: "Required for [Secret] properties via 'dotnet user-secrets'"));
    }
}
