// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;

namespace Deepstaging.Tests.CodeFixes.Config;

public class AddSecretAttributeCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsSecretAttribute()
    {
        const string source =
            """
            namespace TestApp;

            public class SlackSecrets
            {
                public string ApiToken { get; init; } = "";
            }

            [Deepstaging.Config.ConfigProvider(Section = "Slack")]
            [Deepstaging.Config.Exposes<SlackSecrets>]
            public sealed partial class SlackConfigProvider;
            """;

        const string expected =
            """
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

        await AnalyzeAndFixWith<PotentialSecretPropertyAnalyzer, AddSecretAttributeCodeFix>(source)
            .ForDiagnostic(PotentialSecretPropertyAnalyzer.DiagnosticId)
            .ShouldProduce(expected);
    }
}
