// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Projection.Config;
using Deepstaging.Projection.Config.Models;

namespace Deepstaging.Tests.Projection.Config.Models;

public class ConfigModelQueryTests : RoslynTestBase
{
    [Test]
    public async Task QueryConfigModel_ExtractsFullModel()
    {
        var model = SymbolsFor(
                """
                using Deepstaging.Config;

                namespace TestApp;

                public class NotificationSettings
                {
                    public string Channel { get; init; } = "";
                    public int RetryCount { get; init; }
                }

                public class NotificationSecrets
                {
                    [Secret]
                    public string ApiToken { get; init; } = "";
                }

                [ConfigProvider(Section = "Notifications")]
                [Exposes<NotificationSettings>]
                [Exposes<NotificationSecrets>]
                public sealed partial class NotificationConfigProvider;
                """)
            .RequireNamedType("NotificationConfigProvider")
            .QueryConfigModel();

        await Assert.That(model.Namespace).IsEqualTo("TestApp");
        await Assert.That(model.TypeName.Value).IsEqualTo("NotificationConfigProvider");
        await Assert.That(model.Accessibility).IsEqualTo("public");
        await Assert.That(model.Section).IsEqualTo("Notifications");
        await Assert.That(model.HasSecrets).IsTrue();
        await Assert.That(model.ExposedConfigurationTypes.Count).IsEqualTo(2);

        var settings = model.ExposedConfigurationTypes[0];
        await Assert.That(settings.Type.Name).IsEqualTo("NotificationSettings");
        await Assert.That(settings.Properties.Count).IsEqualTo(2);
        await Assert.That(settings.Properties[0].Property.Name).IsEqualTo("Channel");
        await Assert.That(settings.Properties[0].IsSecret).IsFalse();
        await Assert.That(settings.Properties[1].Property.Name).IsEqualTo("RetryCount");

        var secrets = model.ExposedConfigurationTypes[1];
        await Assert.That(secrets.Type.Name).IsEqualTo("NotificationSecrets");
        await Assert.That(secrets.Properties.Count).IsEqualTo(1);
        await Assert.That(secrets.Properties[0].Property.Name).IsEqualTo("ApiToken");
        await Assert.That(secrets.Properties[0].IsSecret).IsTrue();
    }

    [Test]
    public async Task QueryConfigModel_InfersSection()
    {
        var model = SymbolsFor(
                """
                using Deepstaging.Config;

                namespace TestApp;

                public class SlackConfig
                {
                    public string WebhookUrl { get; init; } = "";
                }

                [ConfigProvider]
                [Exposes<SlackConfig>]
                public sealed partial class SlackConfigProvider;
                """)
            .RequireNamedType("SlackConfigProvider")
            .QueryConfigModel();

        await Assert.That(model.Section).IsEqualTo("Slack");
        await Assert.That(model.HasSecrets).IsFalse();
    }
}
