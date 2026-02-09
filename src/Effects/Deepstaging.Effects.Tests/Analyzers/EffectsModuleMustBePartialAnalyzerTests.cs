// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Analyzers;

namespace Deepstaging.Tests.Analyzers;

public class EffectsModuleMustBePartialAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotPartial()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DS0001")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*EmailEffects*partial*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsPartial()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenClassHasNoEffectsModuleAttribute()
    {
        const string source = """
            namespace TestApp;

            public class RegularClass;
            """;

        await AnalyzeWith<EffectsModuleMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsDiagnostic_ForMultipleNonPartialClasses()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }
            public interface ISlackService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public class EmailEffects;

            [Deepstaging.EffectsModule(typeof(ISlackService))]
            public class SlackEffects;
            """;

        await AnalyzeWith<EffectsModuleMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DS0001");
    }
}
