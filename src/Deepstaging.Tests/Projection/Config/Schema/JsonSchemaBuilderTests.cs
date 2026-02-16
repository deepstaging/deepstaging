// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Projection.Config.Models;
using Deepstaging.Projection.Config.Schema;
using Deepstaging.Roslyn;

namespace Deepstaging.Tests.Projection.Config.Schema;

/// <summary>
/// Tests for <see cref="JsonSchemaBuilder"/>.
/// </summary>
public class JsonSchemaBuilderTests
{
    [Test]
    public async Task BuildAppSettingsSchema_ContainsSectionAndProperties()
    {
        var model = new ConfigModel
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
                            IsSecret: false),
                        new ConfigTypePropertyModel(
                            Property: new PropertySnapshot { Name = "RetryCount", Type = "global::System.Int32" },
                            Documentation: DocumentationSnapshot.Empty,
                            IsSecret: false),
                        new ConfigTypePropertyModel(
                            Property: new PropertySnapshot { Name = "ApiKey", Type = "global::System.String" },
                            Documentation: DocumentationSnapshot.Empty,
                            IsSecret: true)
                    ])
            ]
        };

        var schema = JsonSchemaBuilder.BuildAppSettingsSchema(model);

        await Assert.That(schema).Contains("\"$schema\": \"https://json-schema.org/draft-07/schema#\"");
        await Assert.That(schema).Contains("\"Slack\"");
        await Assert.That(schema).Contains("\"SlackConfig\"");
        await Assert.That(schema).Contains("\"WebhookUrl\"");
        await Assert.That(schema).Contains("\"RetryCount\"");
        await Assert.That(schema).Contains("\"string\"");
        await Assert.That(schema).Contains("\"integer\"");
        // Secret property should NOT appear in appsettings schema
        await Assert.That(schema).DoesNotContain("\"ApiKey\"");
    }

    [Test]
    public async Task BuildSecretsSchema_ContainsOnlySecretProperties()
    {
        var model = new ConfigModel
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
                            IsSecret: false),
                        new ConfigTypePropertyModel(
                            Property: new PropertySnapshot { Name = "ApiKey", Type = "global::System.String" },
                            Documentation: DocumentationSnapshot.Empty,
                            IsSecret: true)
                    ])
            ]
        };

        var schema = JsonSchemaBuilder.BuildSecretsSchema(model);

        await Assert.That(schema).Contains("\"ApiKey\"");
        await Assert.That(schema).Contains("\"string\"");
        // Non-secret property should NOT appear in secrets schema
        await Assert.That(schema).DoesNotContain("\"WebhookUrl\"");
    }

    [Test]
    public async Task MapToJsonSchemaType_MapsCorrectly()
    {
        await Assert.That(JsonSchemaBuilder.MapToJsonSchemaType("global::System.String")).IsEqualTo("string");
        await Assert.That(JsonSchemaBuilder.MapToJsonSchemaType("global::System.Int32")).IsEqualTo("integer");
        await Assert.That(JsonSchemaBuilder.MapToJsonSchemaType("global::System.Boolean")).IsEqualTo("boolean");
        await Assert.That(JsonSchemaBuilder.MapToJsonSchemaType("global::System.Double")).IsEqualTo("number");
        await Assert.That(JsonSchemaBuilder.MapToJsonSchemaType("global::System.Guid")).IsEqualTo("string");
        await Assert.That(JsonSchemaBuilder.MapToJsonSchemaType("global::System.Uri")).IsEqualTo("string");
        await Assert.That(JsonSchemaBuilder.MapToJsonSchemaType("global::MyApp.CustomType")).IsEqualTo("object");
    }
}
