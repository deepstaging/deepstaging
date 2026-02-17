// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Testing;
using Deepstaging.Generators;

namespace Deepstaging.Tests.Generators.Config;

/// <summary>
/// Tests for the ConfigGenerator source generator.
/// </summary>
public class ConfigGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task GeneratesWithMethods_ForClassWithInitProperties()
    {
        const string source = """
                              using Deepstaging.Config;

                              namespace TestApp;

                              public sealed record SlackConfig(string ApiKey);

                              [ConfigProvider(Section = "Slack")]
                              [Exposes<SlackConfig>]
                              public partial class SlackConfigProvider;
                              """;

        await GenerateWith<ConfigGenerator>(source)
            .ShouldGenerate()
            .VerifySnapshot();
    }

    [Test]
    public async Task GeneratesMultiExpose_WithSecretProperties()
    {
        const string source = """
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

                                  [Secret]
                                  public string WebhookSecret { get; init; } = "";
                              }

                              [ConfigProvider]
                              [Exposes<NotificationSettings>]
                              [Exposes<NotificationSecrets>]
                              public sealed partial class NotificationConfigProvider;
                              """;

        await GenerateWith<ConfigGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("INotificationConfigProvider")
            .WithFileContaining("NotificationSettings")
            .WithFileContaining("NotificationSecrets")
            .WithFileContaining("AddNotificationConfigProvider")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }
}
