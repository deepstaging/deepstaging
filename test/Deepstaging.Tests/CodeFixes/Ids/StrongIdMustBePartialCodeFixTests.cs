// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.CodeFixes.Ids;

public class StrongIdMustBePartialCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsPartialModifier()
    {
        const string source =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [StrongId]
            public struct UserId;
            """;

        const string expected =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [StrongId]
            public partial struct UserId;
            """;

        await AnalyzeAndFixWith<StrongIdMustBePartialAnalyzer, StructMustBePartialCodeFix>(source)
            .ForDiagnostic("DSID01")
            .ShouldProduce(expected);
    }
}