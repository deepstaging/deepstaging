// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Tests.Analyzers.Config;

/// <summary>
/// Tests for <see cref="ConfigProviderShouldBeSealedAnalyzer"/>.
/// </summary>
public class ConfigProviderShouldBeSealedAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotSealed()
    {
        const string source = """
            namespace TestApp;
            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            public partial class SlackConfigProvider;
        """;

        await AnalyzeWith<ConfigProviderShouldBeSealedAnalyzer>(source)
            .ShouldReportDiagnostic("DSCFG02")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
            .WithMessage("*SlackConfigProvider*sealed*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsSealed()
    {
        const string source = """
            namespace TestApp;
            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            public sealed partial class SlackConfigProvider;
        """;

        await AnalyzeWith<ConfigProviderShouldBeSealedAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
