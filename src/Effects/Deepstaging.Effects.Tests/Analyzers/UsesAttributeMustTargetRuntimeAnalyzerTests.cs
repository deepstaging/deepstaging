// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Tests.Analyzers;

public class UsesAttributeMustTargetRuntimeAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenUsesWithoutRuntime()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;

            [Deepstaging.Uses(typeof(EmailEffects))]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<UsesAttributeMustTargetRuntimeAnalyzer>(source)
            .ShouldReportDiagnostic("DS0003")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*AppRuntime*[Uses]*[Runtime]*");
    }

    [Test]
    public async Task NoDiagnostic_WhenUsesWithRuntime()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;

            [Deepstaging.Runtime]
            [Deepstaging.Uses(typeof(EmailEffects))]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<UsesAttributeMustTargetRuntimeAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoUsesAttribute()
    {
        const string source = """
            namespace TestApp;

            public partial class RegularClass;
            """;

        await AnalyzeWith<UsesAttributeMustTargetRuntimeAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
