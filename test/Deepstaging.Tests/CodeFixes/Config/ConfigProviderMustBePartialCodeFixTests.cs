// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;

namespace Deepstaging.Tests.CodeFixes.Config;

public class ConfigProviderMustBePartialCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsPartialModifier()
    {
        const string source =
            """
            namespace TestApp;

            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            public class SlackConfigProvider;
            """;

        const string expected =
            """
            namespace TestApp;

            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            public partial class SlackConfigProvider;
            """;

        await AnalyzeAndFixWith<ConfigProviderMustBePartialAnalyzer, ClassMustBePartialCodeFix>(source)
            .ForDiagnostic(ConfigProviderMustBePartialAnalyzer.DiagnosticId)
            .ShouldProduce(expected);
    }
}
