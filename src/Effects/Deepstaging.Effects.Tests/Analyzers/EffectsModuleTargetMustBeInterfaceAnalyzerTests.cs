// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Tests.Analyzers;

public class EffectsModuleTargetMustBeInterfaceAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenTargetIsConcreteClass()
    {
        const string source = """
            namespace TestApp;

            public class EmailService { }

            [Deepstaging.EffectsModule(typeof(EmailService))]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleTargetMustBeInterfaceAnalyzer>(source)
            .ShouldReportDiagnostic("DS0004")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
            .WithMessage("*EmailEffects*EmailService*interface*");
    }

    [Test]
    public async Task NoDiagnostic_WhenTargetIsInterface()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleTargetMustBeInterfaceAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenTargetIsDbContext()
    {
        const string source = """
            using Microsoft.EntityFrameworkCore;

            namespace TestApp;

            public class AppDbContext : DbContext { }

            [Deepstaging.EffectsModule(typeof(AppDbContext))]
            public sealed partial class DatabaseEffects;
            """;

        await AnalyzeWith<EffectsModuleTargetMustBeInterfaceAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoEffectsModule()
    {
        const string source = """
            namespace TestApp;

            public sealed partial class RegularClass;
            """;

        await AnalyzeWith<EffectsModuleTargetMustBeInterfaceAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
