// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes.Config;

using Analyzers.Config;
using Deepstaging.Projection.Config;
using Deepstaging.Projection.Config.Schema;
using RoslynAdditionalDocument = Roslyn.AdditionalDocument;

/// <summary>
/// Code fix that generates <c>appsettings.schema.json</c> for a ConfigProvider class.
/// </summary>
[Shared]
[CodeFix(ConfigProviderSchemaAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(GenerateAppSettingsSchemaCodeFix))]
public sealed class GenerateAppSettingsSchemaCodeFix : AdditionalDocumentCodeFix<INamedTypeSymbol>
{
    /// <inheritdoc />
    protected override RoslynAdditionalDocument? CreateDocument(Compilation compilation, ValidSymbol<INamedTypeSymbol> symbol)
    {
        var model = symbol.QueryConfigModel();
        var schema = JsonSchemaBuilder.BuildAppSettingsSchema(model);
        return new RoslynAdditionalDocument("appsettings.schema.json", schema);
    }

    /// <inheritdoc />
    protected override string GetTitle(RoslynAdditionalDocument document, Diagnostic diagnostic) =>
        "Generate appsettings.schema.json";
}