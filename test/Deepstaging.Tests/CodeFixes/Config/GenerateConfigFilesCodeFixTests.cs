// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;

namespace Deepstaging.Tests.CodeFixes.Config;

public class GenerateConfigFilesCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task OffersConfigFileGeneration()
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
            public sealed partial class SlackConfigProvider;
            """;

        await AnalyzeAndFixWith<ConfigProviderSchemaAnalyzer, GenerateConfigFilesCodeFix>(source)
            .ForDiagnostic(ConfigProviderSchemaAnalyzer.DiagnosticId)
            .ShouldOfferFix("Generate configuration files");
    }
}
