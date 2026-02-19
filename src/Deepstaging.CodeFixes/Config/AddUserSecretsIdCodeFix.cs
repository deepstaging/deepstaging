// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes.Config;

using Deepstaging.Projection.Config;

/// <summary>
/// Code fix provider that adds a <c>&lt;UserSecretsId&gt;</c> to <c>deepstaging.props</c>
/// when a <c>[ConfigProvider]</c> exposes <c>[Secret]</c> properties.
/// </summary>
[Shared]
[CodeFix(MissingUserSecretsIdAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddUserSecretsIdCodeFix))]
public sealed class AddUserSecretsIdCodeFix : ProjectCodeFix<INamedTypeSymbol>
{
    /// <inheritdoc />
    protected override CodeAction CreateFix(Project project, ValidSymbol<INamedTypeSymbol> symbol, Diagnostic diagnostic)
    {
        diagnostic.Properties.TryGetValue(ConfigModelQueries.DataDirectoryPropertyName, out var dataDirectory);
        dataDirectory ??= ConfigModelQueries.DefaultDataDirectory;

        return project.ModifyPropsFileAction<DeepstagingProps>(
            "Add UserSecretsId to deepstaging.props",
            dataDirectory,
            doc =>
            {
                doc.SetPropertyGroup(DeepstagingProps.SettingsPropertyGroup, group => group
                    .Property(DeepstagingProps.UserSecretsIdProperty, Guid.NewGuid().ToString(), comment: "Required for [Secret] properties via 'dotnet user-secrets'"));
            });
    }
}