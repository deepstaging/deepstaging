// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Tests.Analyzers.Config;

/// <summary>
/// Tests for <see cref="ExposedTypeHasNoPropertiesAnalyzer"/>.
/// </summary>
public class ExposedTypeHasNoPropertiesAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenExposedTypeHasNoProperties()
    {
        const string source = """
            namespace TestApp;

            public class EmptyConfig;

            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            [Deepstaging.Config.Exposes<EmptyConfig>]
            public partial class SlackConfigProvider;
        """;

        await AnalyzeWith<ExposedTypeHasNoPropertiesAnalyzer>(source)
            .ShouldReportDiagnostic("DSCFG04")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
            .WithMessage("*EmptyConfig*SlackConfigProvider*");
    }

    [Test]
    public async Task NoDiagnostic_WhenExposedTypeHasProperties()
    {
        const string source = """
            namespace TestApp;

            public class SlackConfig
            {
                public string WebhookUrl { get; init; } = "";
            }

            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            [Deepstaging.Config.Exposes<SlackConfig>]
            public partial class SlackConfigProvider;
        """;

        await AnalyzeWith<ExposedTypeHasNoPropertiesAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
