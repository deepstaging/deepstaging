// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Dispatch;

public class DispatchModuleMustBeStaticAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotStatic()
    {
        const string source =
            """
            using Deepstaging.Dispatch;

            namespace TestApp;

            [DispatchModule]
            public partial class Dispatch;
            """;

        await AnalyzeWith<DispatchModuleMustBeStaticAnalyzer>(source)
            .ShouldReportDiagnostic("DSDSP02")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*Dispatch*static*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsStatic()
    {
        const string source =
            """
            using Deepstaging.Dispatch;

            namespace TestApp;

            [DispatchModule]
            public static partial class Dispatch;
            """;

        await AnalyzeWith<DispatchModuleMustBeStaticAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
