// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Testing;
using Deepstaging.Config.Generators;

namespace Deepstaging.Config.Tests;

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

                              [Config]
                              [Exposes<SlackConfig>]
                              public partial class Configuration;
                              """;

        await GenerateWith<ConfigGenerator>(source)
            .ShouldGenerate()
            .WithFileCount(1)
            .VerifySnapshot();
    }
}