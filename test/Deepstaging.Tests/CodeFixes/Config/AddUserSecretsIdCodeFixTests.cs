// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.CodeFixes.Config;

using Deepstaging.Analyzers.Config;

public class AddUserSecretsIdCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task OffersFixForMissingUserSecretsId()
    {
        const string source =
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

        await AnalyzeAndFixWith<MissingUserSecretsIdAnalyzer, AddUserSecretsIdCodeFix>(source)
            .ForDiagnostic(MissingUserSecretsIdAnalyzer.DiagnosticId)
            .ShouldOfferFix("UserSecretsId");
    }
}
