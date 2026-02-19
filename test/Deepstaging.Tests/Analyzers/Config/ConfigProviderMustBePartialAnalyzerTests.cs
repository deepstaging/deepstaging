// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Tests.Analyzers.Config;

/// <summary>
/// Tests for <see cref="ConfigProviderMustBePartialAnalyzer"/>.
/// </summary>
public class ConfigProviderMustBePartialAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotPartial()
    {
        const string source = """
            namespace TestApp;
            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            public class SlackConfigProvider;
        """;

        await AnalyzeWith<ConfigProviderMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DSCFG01")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*SlackConfigProvider*partial*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsPartial()
    {
        const string source = """
            namespace TestApp;
            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            public partial class SlackConfigProvider;
        """;

        await AnalyzeWith<ConfigProviderMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
