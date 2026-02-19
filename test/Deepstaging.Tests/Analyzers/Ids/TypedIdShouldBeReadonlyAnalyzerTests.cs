// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Ids;

public class TypedIdShouldBeReadonlyAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenStructIsNotReadonly()
    {
        const string source =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [TypedId]
            public partial struct UserId;
            """;

        await AnalyzeWith<TypedIdShouldBeReadonlyAnalyzer>(source)
            .ShouldReportDiagnostic("DSID02")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
            .WithMessage("*UserId*readonly*");
    }

    [Test]
    public async Task NoDiagnostic_WhenStructIsReadonly()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [TypedId]
                              public readonly partial struct UserId;
                              """;

        await AnalyzeWith<TypedIdShouldBeReadonlyAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenStructHasNoTypedIdAttribute()
    {
        const string source = """
                              namespace TestApp;

                              public partial struct RegularStruct;
                              """;

        await AnalyzeWith<TypedIdShouldBeReadonlyAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}