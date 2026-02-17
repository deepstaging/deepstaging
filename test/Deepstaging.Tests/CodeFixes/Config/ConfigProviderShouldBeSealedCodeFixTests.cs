// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;

namespace Deepstaging.Tests.CodeFixes.Config;

public class ConfigProviderShouldBeSealedCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsSealedModifier()
    {
        const string source =
            """
            namespace TestApp;

            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            public partial class SlackConfigProvider;
            """;

        const string expected =
            """
            namespace TestApp;

            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            public sealed partial class SlackConfigProvider;
            """;

        await AnalyzeAndFixWith<ConfigProviderShouldBeSealedAnalyzer, ClassShouldBeSealedCodeFix>(source)
            .ForDiagnostic(ConfigProviderShouldBeSealedAnalyzer.DiagnosticId)
            .ShouldProduce(expected);
    }
}
