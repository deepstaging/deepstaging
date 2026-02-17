// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Generators.Writers.Config;
using Deepstaging.Projection.Config;

namespace Deepstaging.Tests.Generators.Config.Writers;

public class ConfigWriterTests : RoslynTestBase
{
    [Test]
    public async Task WriteConfigClass_ProducesInterfaceAndClass()
    {
        var emit = SymbolsFor(
                """
                using Deepstaging.Config;

                namespace TestApp;

                public class SlackConfig
                {
                    public string WebhookUrl { get; init; } = "";
                    public int RetryCount { get; init; }
                }

                [ConfigProvider(Section = "Slack")]
                [Exposes<SlackConfig>]
                public sealed partial class SlackConfigProvider;
                """)
            .RequireNamedType("SlackConfigProvider")
            .QueryConfigModel()
            .WriteConfigClass();

        await Assert.That(emit).IsSuccessful();
        await Verify(emit.Code);
    }

    [Test]
    public async Task WriteConfigClass_WithSecrets_IncludesUserSecrets()
    {
        var emit = SymbolsFor(
                """
                using Deepstaging.Config;

                namespace TestApp;

                public class DbSecrets
                {
                    [Secret]
                    public string ConnectionString { get; init; } = "";
                }

                [ConfigProvider(Section = "Database")]
                [Exposes<DbSecrets>]
                public sealed partial class DatabaseConfigProvider;
                """)
            .RequireNamedType("DatabaseConfigProvider")
            .QueryConfigModel()
            .WriteConfigClass();

        await Assert.That(emit).IsSuccessful();
        await Verify(emit.Code);
    }

    [Test]
    public async Task WriteConfigClass_WithoutSecrets_OmitsUserSecrets()
    {
        var emit = SymbolsFor(
                """
                using Deepstaging.Config;

                namespace TestApp;

                public class AppConfig
                {
                    public string Name { get; init; } = "";
                }

                [ConfigProvider(Section = "App")]
                [Exposes<AppConfig>]
                public sealed partial class AppConfigProvider;
                """)
            .RequireNamedType("AppConfigProvider")
            .QueryConfigModel()
            .WriteConfigClass();

        await Assert.That(emit).IsSuccessful();

        var code = emit.Code;
        await Assert.That(code).Contains("AddJsonFile");
        await Assert.That(code).DoesNotContain("AddUserSecrets");
    }
}
