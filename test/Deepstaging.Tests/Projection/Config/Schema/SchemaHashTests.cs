// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Projection.Config.Models;
using Deepstaging.Projection.Config.Schema;
using Deepstaging.Roslyn;

namespace Deepstaging.Tests.Projection.Config.Schema;

/// <summary>
/// Tests for <see cref="SchemaHash"/>.
/// </summary>
public class SchemaHashTests
{
    [Test]
    public async Task Compute_ReturnsDeterministicHash()
    {
        var model = CreateModel();
        var hash1 = SchemaHash.Compute(model);
        var hash2 = SchemaHash.Compute(model);

        await Assert.That(hash1).IsEqualTo(hash2);
    }

    [Test]
    public async Task Compute_StartsWithPrefix()
    {
        var model = CreateModel();
        var hash = SchemaHash.Compute(model);

        await Assert.That(hash).StartsWith("deepstaging:sha256:");
    }

    [Test]
    public async Task Compute_DifferentModelsProduceDifferentHashes()
    {
        var model1 = CreateModel();
        var model2 = CreateModel() with { Section = "DifferentSection" };

        var hash1 = SchemaHash.Compute(model1);
        var hash2 = SchemaHash.Compute(model2);

        await Assert.That(hash1).IsNotEqualTo(hash2);
    }

    [Test]
    public async Task Extract_ReturnsHashFromSchemaContent()
    {
        const string content = """
            {
              "$schema": "https://json-schema.org/draft-07/schema#",
              "$comment": "deepstaging:sha256:abc123def456"
            }
            """;

        var hash = SchemaHash.Extract(content);

        await Assert.That(hash).IsEqualTo("deepstaging:sha256:abc123def456");
    }

    [Test]
    public async Task Extract_ReturnsNull_WhenNoCommentField()
    {
        const string content = """
            { "$schema": "https://json-schema.org/draft-07/schema#" }
            """;

        var hash = SchemaHash.Extract(content);

        await Assert.That(hash).IsNull();
    }

    [Test]
    public async Task Extract_ReturnsNull_WhenCommentHasWrongPrefix()
    {
        const string content = """
            { "$comment": "some-other-tool:hash123" }
            """;

        var hash = SchemaHash.Extract(content);

        await Assert.That(hash).IsNull();
    }

    [Test]
    public async Task RoundTrip_ComputeAndExtractFromGeneratedSchema()
    {
        var model = CreateModel();
        var schema = JsonSchemaBuilder.BuildAppSettingsSchema(model);
        var expectedHash = SchemaHash.Compute(model);
        var extractedHash = SchemaHash.Extract(schema);

        await Assert.That(extractedHash).IsEqualTo(expectedHash);
    }

    private static ConfigModel CreateModel() => new()
    {
        Namespace = "TestApp",
        TypeName = "SlackConfigProvider",
        Accessibility = "public",
        Section = "Slack",
        ExposedConfigurationTypes =
        [
            new ConfigTypeModel(
                Type: new TypeSnapshot { Name = "SlackConfig", CodeName = "global::TestApp.SlackConfig" },
                Properties:
                [
                    new ConfigTypePropertyModel(
                        Property: new PropertySnapshot { Name = "WebhookUrl", Type = "global::System.String" },
                        Documentation: DocumentationSnapshot.Empty,
                        IsSecret: false)
                ])
        ]
    };
}
