// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Ids.Analyzers;
using Deepstaging.Ids.CodeFixes;

namespace Deepstaging.Ids.Tests.Analyzers;

public class StrongIdMustBePartialCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsPartialModifier()
    {
        const string source = """
            using Deepstaging.Ids;

            namespace TestApp;

            [StrongId]
            public struct UserId;
            """;

        const string expected = """
            using Deepstaging.Ids;

            namespace TestApp;

            [StrongId]
            public partial struct UserId;
            """;

        await AnalyzeAndFixWith<StrongIdMustBePartialAnalyzer, StrongIdMustBePartialCodeFix>(source)
            .ForDiagnostic("ID0001")
            .ShouldProduce(expected);
    }
}
