using Deepstaging.Analyzers;

namespace Deepstaging.Tests.Analyzers;

public class EffectsModuleShouldBeSealedAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotSealed()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleShouldBeSealedAnalyzer>(source)
            .ShouldReportDiagnostic("DS0009")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
            .WithMessage("*EmailEffects*sealed*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsSealed()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleShouldBeSealedAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsStatic()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public static partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleShouldBeSealedAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenClassHasNoEffectsModuleAttribute()
    {
        const string source = """
            namespace TestApp;

            public partial class RegularClass;
            """;

        await AnalyzeWith<EffectsModuleShouldBeSealedAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
