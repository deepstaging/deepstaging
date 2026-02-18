// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Effects;

public class UsesAttributeMustTargetRuntimeAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenUsesWithoutRuntime()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;

            [Deepstaging.Effects.Uses(typeof(EmailEffects))]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<UsesAttributeMustTargetRuntimeAnalyzer>(source)
            .ShouldReportDiagnostic("DSRT02")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*AppRuntime*[Uses]*[Runtime]*");
    }

    [Test]
    public async Task NoDiagnostic_WhenUsesWithRuntime()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;

            [Deepstaging.Effects.Runtime]
            [Deepstaging.Effects.Uses(typeof(EmailEffects))]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<UsesAttributeMustTargetRuntimeAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoUsesAttribute()
    {
        const string source =
            """
            namespace TestApp;

            public partial class RegularClass;
            """;

        await AnalyzeWith<UsesAttributeMustTargetRuntimeAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}