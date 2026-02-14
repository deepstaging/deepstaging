// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Config.Projection.Attributes;
using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Config.Tests.Projection.Attributes;

public class ExposesAttributeQueryTests : RoslynTestBase
{
    [Test]
    public async Task ExposesAttributeQuery_ConfigurationType_ReturnsExpectedType()
    {
        var attribute = SymbolsFor(
                """
                namespace TestApp;

                public sealed record SlackConfig(string ApiKey);

                [Deepstaging.Config.Exposes<SlackConfig>]
                public partial class Configuration;
                """
            )
            .RequireNamedType("Configuration")
            .GetAttribute(typeof(ExposesAttribute<>))
            .Map(attr => attr.AsQuery<ExposesAttributeQuery>())
            .OrThrow("Expected Configuration to have ExposesAttribute.");

        await Assert.That(attribute.ConfigurationType).IsRecordSymbol();
        await Assert.That(attribute.ConfigurationType.Name).IsEqualTo("SlackConfig");

        var props = attribute.ConfigurationType.QueryProperties().GetAll();

        await Assert.That(props.Length).IsEqualTo(1);
        await Assert.That(props[0].Name).IsEqualTo("ApiKey");
        await Assert.That(props[0].ReturnType.FullyQualifiedName).IsEqualTo("string");
    }
}
