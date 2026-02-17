// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Tests.Analyzers.Config;

/// <summary>
/// Tests for <see cref="PotentialSecretPropertyAnalyzer"/>.
/// </summary>
public class PotentialSecretPropertyAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenPropertyLooksLikeSecret()
    {
        const string source = """
            namespace TestApp;

            public class SlackConfig
            {
                public string ApiKey { get; init; } = "";
            }

            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            [Deepstaging.Config.Exposes<SlackConfig>]
            public partial class SlackConfigProvider;
        """;

        await AnalyzeWith<PotentialSecretPropertyAnalyzer>(source)
            .ShouldReportDiagnostic("CFG005")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
            .WithMessage("*ApiKey*SlackConfig*");
    }

    [Test]
    public async Task NoDiagnostic_WhenPropertyIsMarkedAsSecret()
    {
        const string source = """
            namespace TestApp;

            public class SlackConfig
            {
                [Deepstaging.Config.Secret]
                public string ApiKey { get; init; } = "";
            }

            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            [Deepstaging.Config.Exposes<SlackConfig>]
            public partial class SlackConfigProvider;
        """;

        await AnalyzeWith<PotentialSecretPropertyAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenPropertyNameIsNotSuspicious()
    {
        const string source = """
            namespace TestApp;

            public class SlackConfig
            {
                public string WebhookUrl { get; init; } = "";
                public int RetryCount { get; init; }
            }

            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            [Deepstaging.Config.Exposes<SlackConfig>]
            public partial class SlackConfigProvider;
        """;

        await AnalyzeWith<PotentialSecretPropertyAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsDiagnostic_ForVariousSecretPatterns()
    {
        const string source = """
            namespace TestApp;

            public class DbConfig
            {
                public string ConnectionString { get; init; } = "";
            }

            [Deepstaging.Config.ConfigProvider(Section = "Db")]
            [Deepstaging.Config.Exposes<DbConfig>]
            public partial class DbConfigProvider;
        """;

        await AnalyzeWith<PotentialSecretPropertyAnalyzer>(source)
            .ShouldReportDiagnostic("CFG005")
            .WithMessage("*ConnectionString*DbConfig*");
    }
}
