// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.CodeFixes;

using Projection.Config;
using Projection.Config.Schema;
using AdditionalDocument = Roslyn.AdditionalDocument;

/// <summary>
/// Code fix that generates <c>secrets.schema.json</c> for a ConfigProvider class.
/// </summary>
[Shared]
[CodeFix(ConfigProviderSchemaAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(GenerateSecretsSchemaCodeFix))]
public sealed class GenerateSecretsSchemaCodeFix : AdditionalDocumentCodeFix<INamedTypeSymbol>
{
    /// <inheritdoc />
    protected override AdditionalDocument? CreateDocument(Compilation compilation, ValidSymbol<INamedTypeSymbol> symbol)
    {
        var model = symbol.QueryConfigModel();

        var hasSecrets = model.ExposedConfigurationTypes
            .Any(ct => ct.Properties.Any(p => p.IsSecret));

        if (!hasSecrets)
            return null;

        var schema = JsonSchemaBuilder.BuildSecretsSchema(model);
        return new AdditionalDocument("secrets.schema.json", schema);
    }

    /// <inheritdoc />
    protected override string GetTitle(AdditionalDocument document, Diagnostic diagnostic) =>
        "Generate secrets.schema.json";
}