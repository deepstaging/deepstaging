// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Effects;

public class EffectsModuleMustBePartialAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotPartial()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DSEFX01")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*EmailEffects*partial*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsPartial()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenClassHasNoEffectsModuleAttribute()
    {
        const string source =
            """
            namespace TestApp;

            public class RegularClass;
            """;

        await AnalyzeWith<EffectsModuleMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsDiagnostic_ForMultipleNonPartialClasses()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }
            public interface ISlackService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public class EmailEffects;

            [Deepstaging.Effects.EffectsModule(typeof(ISlackService))]
            public class SlackEffects;
            """;

        await AnalyzeWith<EffectsModuleMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DSEFX01");
    }
}