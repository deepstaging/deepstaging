// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Tests.Analyzers.Config;

/// <summary>
/// Tests for <see cref="ConfigProviderSchemaAnalyzer"/>.
/// </summary>
public class ConfigProviderSchemaAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenConfigProviderExists()
    {
        const string source = """
            namespace TestApp;
            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            public sealed partial class SlackConfigProvider;
        """;

        await AnalyzeWith<ConfigProviderSchemaAnalyzer>(source)
            .ShouldReportDiagnostic("CFG006")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Info)
            .WithMessage("*SlackConfigProvider*schema*");
    }
}
