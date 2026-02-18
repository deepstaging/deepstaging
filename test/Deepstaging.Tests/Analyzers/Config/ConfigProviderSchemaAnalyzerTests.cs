// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;
using Deepstaging.Projection.Config.Models;
using Deepstaging.Projection.Config.Schema;
using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Tests.Analyzers.Config;

/// <summary>
/// Tests for <see cref="ConfigProviderSchemaAnalyzer"/>.
/// </summary>
public class ConfigProviderSchemaAnalyzerTests : RoslynTestBase
{
    private const string Source = """
        namespace TestApp;
        [Deepstaging.Config.ConfigProvider(Section = "Slack")]
        public sealed partial class SlackConfigProvider;
    """;

    [Test]
    public async Task ReportsMissingDiagnostic_WhenNoSchemaFilesExist()
    {
        await AnalyzeWith<ConfigProviderSchemaAnalyzer>(Source)
            .ShouldReportDiagnostic("DSCFG06")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Info)
            .WithMessage("*SlackConfigProvider*generate*");
    }

    [Test]
    public async Task ReportsStaleDiagnostic_WhenSchemaHashMismatches()
    {
        await AnalyzeWith<ConfigProviderSchemaAnalyzer>(Source)
            .WithAdditionalText("deepstaging.schema.json", """
                { "$comment": "deepstaging:sha256:0000000000000000000000000000000000000000000000000000000000000000" }
            """)
            .ShouldReportDiagnostic("DSCFG06")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
            .WithMessage("*SlackConfigProvider*out of date*");
    }

    [Test]
    public async Task ReportsNoDiagnostic_WhenSchemaIsUpToDate()
    {
        // Build the hash that the analyzer will compute for this model
        var model = new ConfigModel
        {
            Namespace = "TestApp",
            TypeName = "SlackConfigProvider",
            Accessibility = "public",
            Section = "Slack",
            ExposedConfigurationTypes = []
        };

        var hash = SchemaHash.Compute(model);

        await AnalyzeWith<ConfigProviderSchemaAnalyzer>(Source)
            .WithAdditionalText("deepstaging.schema.json", $$"""
                { "$comment": "{{hash}}" }
            """)
            .ShouldHaveNoDiagnostics();
    }
}
