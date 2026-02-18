// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Ids;

public class StrongIdMustBePartialAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenStructIsNotPartial()
    {
        const string source =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [StrongId]
            public struct UserId;
            """;

        await AnalyzeWith<StrongIdMustBePartialAnalyzer>(source)
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

            [StrongId]
            public partial struct UserId;
            """;

        await AnalyzeWith<StrongIdMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenStructHasNoStrongIdAttribute()
    {
        const string source =
            """
            namespace TestApp;

            public struct RegularStruct;
            """;

        await AnalyzeWith<StrongIdMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsDiagnostic_ForMultipleNonPartialStructs()
    {
        const string source =
            """
            using Deepstaging.Ids;

            namespace TestApp;

            [StrongId]
            public struct UserId;

            [StrongId]
            public struct OrderId;
            """;

        await AnalyzeWith<StrongIdMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DSID01");
    }
}