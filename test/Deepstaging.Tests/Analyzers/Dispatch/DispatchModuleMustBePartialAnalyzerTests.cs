// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Dispatch;

public class DispatchModuleMustBePartialAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotPartial()
    {
        const string source =
            """
            using Deepstaging.Dispatch;

            namespace TestApp;

            [DispatchModule]
            public static class Dispatch;
            """;

        await AnalyzeWith<DispatchModuleMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DSDSP01")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*Dispatch*partial*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsPartial()
    {
        const string source =
            """
            using Deepstaging.Dispatch;

            namespace TestApp;

            [DispatchModule]
            public static partial class Dispatch;
            """;

        await AnalyzeWith<DispatchModuleMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
