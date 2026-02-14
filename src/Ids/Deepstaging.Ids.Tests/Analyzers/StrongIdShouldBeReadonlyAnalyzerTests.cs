// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Ids.Analyzers;

namespace Deepstaging.Ids.Tests.Analyzers;

public class StrongIdShouldBeReadonlyAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenStructIsNotReadonly()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [StrongId]
                              public partial struct UserId;
                              """;

        await AnalyzeWith<StrongIdShouldBeReadonlyAnalyzer>(source)
            .ShouldReportDiagnostic("ID0002")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
            .WithMessage("*UserId*readonly*");
    }

    [Test]
    public async Task NoDiagnostic_WhenStructIsReadonly()
    {
        const string source = """
                              using Deepstaging.Ids;

                              namespace TestApp;

                              [StrongId]
                              public readonly partial struct UserId;
                              """;

        await AnalyzeWith<StrongIdShouldBeReadonlyAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenStructHasNoStrongIdAttribute()
    {
        const string source = """
                              namespace TestApp;

                              public partial struct RegularStruct;
                              """;

        await AnalyzeWith<StrongIdShouldBeReadonlyAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
