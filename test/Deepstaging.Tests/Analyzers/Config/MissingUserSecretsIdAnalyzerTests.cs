// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Tests.Analyzers.Config;

/// <summary>
/// Tests for <see cref="MissingUserSecretsIdAnalyzer"/>.
/// </summary>
public class MissingUserSecretsIdAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenSecretPropertiesExist_AndNoUserSecretsId()
    {
        const string source = """
            namespace TestApp;
            public class SlackSecrets
            {
                [Deepstaging.Config.Secret]
                public string ApiToken { get; init; } = "";
            }
            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            [Deepstaging.Config.Exposes<SlackSecrets>]
            public sealed partial class SlackConfigProvider;
        """;

        await AnalyzeWith<MissingUserSecretsIdAnalyzer>(source)
            .ShouldReportDiagnostic("CFG007")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
            .WithMessage("*SlackConfigProvider*UserSecretsId*");
    }

    [Test]
    public async Task NoDiagnostic_WhenNoSecretProperties()
    {
        const string source = """
            namespace TestApp;
            public class SlackConfig
            {
                public string WebhookUrl { get; init; } = "";
            }
            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            [Deepstaging.Config.Exposes<SlackConfig>]
            public sealed partial class SlackConfigProvider;
        """;

        await AnalyzeWith<MissingUserSecretsIdAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
