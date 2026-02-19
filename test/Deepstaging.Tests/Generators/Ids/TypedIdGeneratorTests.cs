// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Generators.Ids;

using Deepstaging.Generators;

/// <summary>
/// Tests for the TypedIdGenerator source generator.
/// </summary>
public class TypedIdGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task GeneratesGuidId_WithDefaultSettings()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [TypedId]
                              public partial struct UserId;
                              """;

        await GenerateWith<TypedIdGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public partial struct UserId")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }

    [Test]
    public async Task GeneratesIntId_WithBackingType()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [TypedId(BackingType = BackingType.Int)]
                              public partial struct OrderId;
                              """;

        await GenerateWith<TypedIdGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public partial struct OrderId")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }

    [Test]
    public async Task GeneratesStringId_WithBackingType()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [TypedId(BackingType = BackingType.String)]
                              public partial struct Sku;
                              """;

        await GenerateWith<TypedIdGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public partial struct Sku")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }

    [Test]
    public async Task GeneratesLongId_WithBackingType()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [TypedId(BackingType = BackingType.Long)]
                              public partial struct TransactionId;
                              """;

        await GenerateWith<TypedIdGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public partial struct TransactionId")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }

    [Test]
    public async Task GeneratesGuidId_WithAllConverters()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [TypedId(Converters = IdConverters.JsonConverter | IdConverters.EfCoreValueConverter)]
                              public partial struct CustomerId;
                              """;

        await GenerateWith<TypedIdGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public partial struct CustomerId")
            .WithFileContaining("CustomerIdTypeConverter")
            .WithFileContaining("CustomerIdSystemTextJsonConverter")
            .WithFileContaining("EfCoreValueConverter")
            .WithNoDiagnostics()
            .CompilesSuccessfully()
            .VerifySnapshot();
    }
}