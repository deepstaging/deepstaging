// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Testing;
using Deepstaging.Ids.Generators;

namespace Deepstaging.Ids.Tests;

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
            .WithFileCount(1)
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
            .WithFileCount(1)
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
            .WithFileCount(1)
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
            .WithFileCount(1)
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
            .WithFileCount(1)
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

    #region User Template Override

    [Test]
    public async Task UsesUserTemplate_WhenProvided()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [StrongId]
                              public partial struct OrderId;
                              """;

        const string template = """
                                namespace TestApp;
                                public partial struct {{ TypeName }}
                                {
                                    // Custom user template
                                    public System.Guid Value { get; }
                                    public {{ TypeName }}(System.Guid value) => Value = value;
                                }
                                """;

        await GenerateWith<StrongIdGenerator>(source)
            .WithAdditionalText("Templates/Deepstaging.Ids/StrongId.scriban-cs", template)
            .ShouldGenerate()
            .WithFileCount(1)
            .WithFileContaining("Custom user template")
            .WithFileContaining("public partial struct OrderId")
            .WithNoDiagnostics();
    }

    [Test]
    public async Task UsesDefaultEmit_WhenNoUserTemplate()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [StrongId]
                              public partial struct ProductId;
                              """;

        await GenerateWith<StrongIdGenerator>(source)
            .ShouldGenerate()
            .WithFileCount(1)
            .WithFileContaining("public partial struct ProductId")
            .WithoutFileContaining("Custom user template")
            .WithNoDiagnostics();
    }

    #endregion
}
