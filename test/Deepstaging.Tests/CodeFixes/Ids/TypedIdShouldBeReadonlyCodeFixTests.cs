// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.CodeFixes.Ids;

public class TypedIdShouldBeReadonlyCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsReadonlyModifier()
    {
        const string source =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [TypedId]
            public partial struct UserId;
            """;

        const string expected =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [TypedId]
            public readonly partial struct UserId;
            """;

        await AnalyzeAndFixWith<TypedIdShouldBeReadonlyAnalyzer, StructShouldBeReadonlyCodeFix>(source)
            .ForDiagnostic("DSID02")
            .ShouldProduce(expected);
    }
}