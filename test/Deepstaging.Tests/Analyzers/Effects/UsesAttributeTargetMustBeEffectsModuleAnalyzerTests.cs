// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Effects;

public class UsesAttributeTargetMustBeEffectsModuleAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenTargetLacksEffectsModule()
    {
        const string source =
            """
            namespace TestApp;

            public class NotAnEffectsModule;

            [Deepstaging.Effects.Runtime]
            [Deepstaging.Effects.Uses(typeof(NotAnEffectsModule))]
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
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            [Deepstaging.Effects.Runtime]
            [Deepstaging.Effects.Uses(typeof(EmailModule))]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<UsesAttributeTargetMustBeEffectsModuleAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoUsesAttribute()
    {
        const string source =
            """
            namespace TestApp;

            [Deepstaging.Effects.Runtime]
            public partial class EmptyRuntime;
            """;

        await AnalyzeWith<UsesAttributeTargetMustBeEffectsModuleAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsDiagnostic_WhenOneOfMultipleTargetsLacksEffectsModule()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            public class NotAModule;

            [Deepstaging.Effects.Runtime]
            [Deepstaging.Effects.Uses(typeof(EmailModule))]
            [Deepstaging.Effects.Uses(typeof(NotAModule))]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<UsesAttributeTargetMustBeEffectsModuleAnalyzer>(source)
            .ShouldReportDiagnostic("DS0008")
            .WithMessage("*NotAModule*");
    }
}