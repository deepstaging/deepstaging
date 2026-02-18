// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Effects;

public class RuntimeMustBePartialAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotPartial()
    {
        const string source =
            """
            namespace TestApp;

            [Deepstaging.Effects.Runtime]
            public class AppRuntime;
            """;

        await AnalyzeWith<RuntimeMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DSRT01")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*AppRuntime*partial*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsPartial()
    {
        const string source =
            """
            namespace TestApp;

            [Deepstaging.Effects.Runtime]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<RuntimeMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenClassHasNoRuntimeAttribute()
    {
        const string source =
            """
            namespace TestApp;

            public class RegularClass;
            """;

        await AnalyzeWith<RuntimeMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}