// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.CodeFixes.Ids;

public class TypedIdMustBePartialCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsPartialModifier()
    {
        const string source =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [TypedId]
            public struct UserId;
            """;

        const string expected =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [TypedId]
            public partial struct UserId;
            """;

        await AnalyzeAndFixWith<TypedIdMustBePartialAnalyzer, StructMustBePartialCodeFix>(source)
            .ForDiagnostic("DSID01")
            .ShouldProduce(expected);
    }
}