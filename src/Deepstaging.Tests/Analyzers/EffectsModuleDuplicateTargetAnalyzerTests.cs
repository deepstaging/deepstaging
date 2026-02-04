using Deepstaging.Analyzers;

namespace Deepstaging.Tests.Analyzers;

public class EffectsModuleDuplicateTargetAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenDuplicateTargetType()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleDuplicateTargetAnalyzer>(source)
            .ShouldReportDiagnostic("DS0005")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*EmailEffects*IEmailService*");
    }

    [Test]
    public async Task NoDiagnostic_WhenDifferentTargetTypes()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }
            public interface ISlackService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            [Deepstaging.EffectsModule(typeof(ISlackService))]
            public sealed partial class NotificationEffects;
            """;

        await AnalyzeWith<EffectsModuleDuplicateTargetAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenSingleEffectsModule()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleDuplicateTargetAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoEffectsModule()
    {
        const string source = """
            namespace TestApp;

            public sealed partial class RegularClass;
            """;

        await AnalyzeWith<EffectsModuleDuplicateTargetAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
