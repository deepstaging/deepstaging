// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Effects;

public class AvailableEffectsModuleAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task NoDiagnostic_WhenAllModulesAreUsed()
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

        await AnalyzeWith<AvailableEffectsModuleAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsDiagnostic_WhenModuleIsNotUsed()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            [Deepstaging.Effects.Runtime]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<AvailableEffectsModuleAnalyzer>(source)
            .ShouldReportDiagnostic("DSRT04")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Info)
            .WithMessage("*EmailModule*AppRuntime*");
    }

    [Test]
    public async Task ReportsMultipleDiagnostics_WhenMultipleModulesAreUnused()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }
            public interface ISlackService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            [Deepstaging.Effects.EffectsModule(typeof(ISlackService))]
            public sealed partial class SlackModule;

            [Deepstaging.Effects.Runtime]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<AvailableEffectsModuleAnalyzer>(source)
            .ShouldHaveDiagnostics()
            .WithErrorCode("DSRT04");
    }

    [Test]
    public async Task NoDiagnostic_WhenNoRuntimeExists()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;
            """;

        await AnalyzeWith<AvailableEffectsModuleAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsOnlyUnusedModules_WhenSomeAreUsed()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }
            public interface ISlackService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            [Deepstaging.Effects.EffectsModule(typeof(ISlackService))]
            public sealed partial class SlackModule;

            [Deepstaging.Effects.Runtime]
            [Deepstaging.Effects.Uses(typeof(EmailModule))]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<AvailableEffectsModuleAnalyzer>(source)
            .ShouldReportDiagnostic("DSRT04")
            .WithMessage("*SlackModule*AppRuntime*");
    }
}
