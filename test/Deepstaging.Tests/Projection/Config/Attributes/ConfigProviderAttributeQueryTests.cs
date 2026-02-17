// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Projection.Config.Attributes;
using Deepstaging.Roslyn;

namespace Deepstaging.Tests.Projection.Config.Attributes;

public class ConfigProviderAttributeQueryTests : RoslynTestBase
{
    [Test]
    public async Task GetSectionName_ReturnsExplicitSection()
    {
        var symbols = SymbolsFor(
            """
            namespace TestApp;

            [Deepstaging.Config.ConfigProvider(Section = "MySlack")]
            public partial class SlackConfigProvider;
            """);

        var type = symbols.RequireNamedType("SlackConfigProvider");
        var query = type.GetAttribute<ConfigProviderAttribute>()
            .Map(attr => attr.AsQuery<ConfigProviderAttributeQuery>())
            .OrThrow();

        await Assert.That(query.GetSectionName(type)).IsEqualTo("MySlack");
    }

    [Test]
    public async Task GetSectionName_InfersFromClassName()
    {
        var symbols = SymbolsFor(
            """
            namespace TestApp;

            [Deepstaging.Config.ConfigProvider]
            public partial class SlackConfigProvider;
            """);

        var type = symbols.RequireNamedType("SlackConfigProvider");
        var query = type.GetAttribute<ConfigProviderAttribute>()
            .Map(attr => attr.AsQuery<ConfigProviderAttributeQuery>())
            .OrThrow();

        await Assert.That(query.GetSectionName(type)).IsEqualTo("Slack");
    }

    [Test]
    public async Task GetSectionName_ReturnsEmpty_WhenNotInferrable()
    {
        var symbols = SymbolsFor(
            """
            namespace TestApp;

            [Deepstaging.Config.ConfigProvider]
            public partial class MySettings;
            """);

        var type = symbols.RequireNamedType("MySettings");
        var query = type.GetAttribute<ConfigProviderAttribute>()
            .Map(attr => attr.AsQuery<ConfigProviderAttributeQuery>())
            .OrThrow();

        await Assert.That(query.GetSectionName(type)).IsEqualTo(string.Empty);
    }
}
