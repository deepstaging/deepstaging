// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes.Config;

using Analyzers.Config;
using Deepstaging.Projection.Config;
using Deepstaging.Projection.Config.Schema;
using Projection.Config.Models;

/// <summary>
/// Code fix that generates all configuration files for a ConfigProvider class in a single action.
/// Writes schema files (overwrite), syncs settings templates (preserving user values, pruning removed keys),
/// and .gitignore entries.
/// Properties go into <c>deepstaging.props</c>; file nesting ItemGroups go into <c>deepstaging.targets</c>.
/// </summary>
[Shared]
[CodeFix(ConfigProviderSchemaAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(GenerateConfigFilesCodeFix))]
public sealed class GenerateConfigFilesCodeFix : ProjectCodeFix<INamedTypeSymbol>
{
    private static readonly string[] Environments = ["Development", "Staging", "Production"];

    private readonly struct Filenames(ConfigModel model)
    {
        private string Qualified(string filename) => $"{model.DataDirectory}/{filename}";
        public string GitIgnore => Qualified(".gitignore");
        public string Props => Qualified("deepstaging.props");
        public string Secrets => Qualified("user-secrets.json");
        public string SettingsSchema => Qualified("deepstaging.schema.json");
        public string SecretsSchema => Qualified("user-secrets.schema.json");
        public string Settings(string? env = null) => env is null ? Qualified("deepstaging.settings.json") : Qualified($"deepstaging.settings.{env}.json");
    }

    /// <inheritdoc />
    protected override CodeAction CreateFix(Project project, ValidSymbol<INamedTypeSymbol> symbol, Diagnostic diagnostic)
    {
        diagnostic.Properties.TryGetValue(ConfigModelQueries.DataDirectoryPropertyName, out var dataDirectory);
        dataDirectory ??= ConfigModelQueries.DefaultDataDirectory;

        var model = symbol.QueryConfigModel(dataDirectory);
        var filenames = new Filenames(model);

        var settingsTemplate = SettingsBuilder.BuildSettings(model, filenames.SettingsSchema);
        var secretesTemplate = SettingsBuilder.BuildSecrets(model, filenames.SecretsSchema);

        return project
            .FileActions("Generate configuration files")
            .ModifyPropsFile<DeepstagingProps>(model.DataDirectory, doc =>
            {
                doc.SetPropertyGroup(DeepstagingProps.SettingsPropertyGroup, group => group
                    .Comment("General Deepstaging Settings")
                    .If(model.HasSecrets, b => b
                        .Property(DeepstagingProps.UserSecretsIdProperty, value: Guid.NewGuid().ToString(), "Required for [Secret] properties via 'dotnet user-secrets'")));
            })
            .ModifyPropsFile<DeepstagingTargets>(model.DataDirectory, doc =>
            {
                doc.SetItemGroup(DeepstagingTargets.FileNestingItemGroup, items => items
                    .Comment("Nest configuration files under the props file for better organization in IDEs")
                    .Item("None", "Update", pattern: filenames.Settings(), m => m.Set("DependentUpon", filenames.Props))
                    .Item("None", "Update", filenames.SettingsSchema, m => m.Set("DependentUpon", filenames.Props))
                    .WithEach(Environments, (b, env) => b
                        .Item("None", "Update", pattern: filenames.Settings(env), m => m.Set("DependentUpon", value: filenames.Settings())))
                    .If(model.HasSecrets, b => b
                        .Item("None", "Update", filenames.SecretsSchema, m => m.Set("DependentUpon", filenames.Props))
                        .Item("None", "Update", filenames.Secrets, m => m.Set("DependentUpon", filenames.Props))));
            })
            .Write(filenames.SettingsSchema, content: JsonSchemaBuilder.BuildAppSettingsSchema(model))
            .SyncJsonFile(relativePath: filenames.Settings(), settingsTemplate)
            .WithEach(Environments, (b, env) => b
                .SyncJsonFile(relativePath: filenames.Settings(env), settingsTemplate))
            .If(model.HasSecrets, b => b
                .Write(filenames.SecretsSchema, content: JsonSchemaBuilder.BuildSecretsSchema(model))
                .SyncJsonFile(filenames.Secrets, secretesTemplate)
                .AppendLine(filenames.GitIgnore, filenames.Secrets))
            .ToCodeAction();
    }
}