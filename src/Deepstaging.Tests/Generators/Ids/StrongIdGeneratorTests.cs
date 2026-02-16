// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Generators.Ids;

using Deepstaging.Generators;

/// <summary>
/// Tests for the StrongIdGenerator source generator.
/// </summary>
public class StrongIdGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task GeneratesGuidId_WithDefaultSettings()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [StrongId]
                              public partial struct UserId;
                              """;

        await GenerateWith<StrongIdGenerator>(source)
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

                              [StrongId(BackingType = BackingType.Int)]
                              public partial struct OrderId;
                              """;

        await GenerateWith<StrongIdGenerator>(source)
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

                              [StrongId(BackingType = BackingType.String)]
                              public partial struct Sku;
                              """;

        await GenerateWith<StrongIdGenerator>(source)
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

                              [StrongId(BackingType = BackingType.Long)]
                              public partial struct TransactionId;
                              """;

        await GenerateWith<StrongIdGenerator>(source)
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

                              [StrongId(Converters = IdConverters.All)]
                              public partial struct CustomerId;
                              """;

        await GenerateWith<StrongIdGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public partial struct CustomerId")
            .WithFileContaining("CustomerIdTypeConverter")
            .WithFileContaining("CustomerIdSystemTextJsonConverter")
            .WithFileContaining("EfCoreValueConverter")
            .WithFileContaining("DapperTypeHandler")
            .WithFileContaining("CustomerIdNewtonsoftJsonConverter")
            .WithNoDiagnostics()
            .CompilesSuccessfully()
            .VerifySnapshot();
    }
}