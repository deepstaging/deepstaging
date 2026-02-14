// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Tests.Analyzers;

public class UsesAttributeTargetMustBeEffectsModuleAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenTargetLacksEffectsModule()
    {
        const string source = """
            namespace TestApp;

            public class NotAnEffectsModule;

            [Deepstaging.Runtime]
            [Deepstaging.Uses(typeof(NotAnEffectsModule))]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<UsesAttributeTargetMustBeEffectsModuleAnalyzer>(source)
            .ShouldReportDiagnostic("DS0008")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*NotAnEffectsModule*[EffectsModule]*");
    }

    [Test]
    public async Task NoDiagnostic_WhenTargetHasEffectsModule()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            [Deepstaging.Runtime]
            [Deepstaging.Uses(typeof(EmailModule))]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<UsesAttributeTargetMustBeEffectsModuleAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoUsesAttribute()
    {
        const string source = """
            namespace TestApp;

            [Deepstaging.Runtime]
            public partial class EmptyRuntime;
            """;

        await AnalyzeWith<UsesAttributeTargetMustBeEffectsModuleAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsDiagnostic_WhenOneOfMultipleTargetsLacksEffectsModule()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            public class NotAModule;

            [Deepstaging.Runtime]
            [Deepstaging.Uses(typeof(EmailModule))]
            [Deepstaging.Uses(typeof(NotAModule))]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<UsesAttributeTargetMustBeEffectsModuleAnalyzer>(source)
            .ShouldReportDiagnostic("DS0008")
            .WithMessage("*NotAModule*");
    }
}
