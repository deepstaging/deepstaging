// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Ids;

public class TypedIdMustBePartialAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenStructIsNotPartial()
    {
        const string source =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [TypedId]
            public struct UserId;
            """;

        await AnalyzeWith<TypedIdMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DSID01")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*UserId*partial*");
    }

    [Test]
    public async Task NoDiagnostic_WhenStructIsPartial()
    {
        const string source =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [TypedId]
            public partial struct UserId;
            """;

        await AnalyzeWith<TypedIdMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenStructHasNoTypedIdAttribute()
    {
        const string source =
            """
            namespace TestApp;

            public struct RegularStruct;
            """;

        await AnalyzeWith<TypedIdMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsDiagnostic_ForMultipleNonPartialStructs()
    {
        const string source =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [TypedId]
            public struct UserId;

            [TypedId]
            public struct OrderId;
            """;

        await AnalyzeWith<TypedIdMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DSID01");
    }
}