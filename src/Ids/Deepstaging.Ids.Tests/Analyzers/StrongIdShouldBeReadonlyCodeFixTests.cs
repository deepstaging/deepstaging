// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Ids.Analyzers;
using Deepstaging.Ids.CodeFixes;

namespace Deepstaging.Ids.Tests.Analyzers;

public class StrongIdShouldBeReadonlyCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsReadonlyModifier()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [StrongId]
                              public partial struct UserId;
                              """;

        const string expected = """
                                using Deepstaging.Ids;

                                namespace TestApp;

                                [StrongId]
                                public readonly partial struct UserId;
                                """;

        await AnalyzeAndFixWith<StrongIdShouldBeReadonlyAnalyzer, StrongIdShouldBeReadonlyCodeFix>(source)
            .ForDiagnostic("ID0002")
            .ShouldProduce(expected);
    }
}
