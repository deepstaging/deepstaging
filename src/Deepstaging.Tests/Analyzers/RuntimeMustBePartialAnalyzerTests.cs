using Deepstaging.Analyzers;

namespace Deepstaging.Tests.Analyzers;

public class RuntimeMustBePartialAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotPartial()
    {
        const string source = """
            namespace TestApp;

            [Deepstaging.Runtime]
            public class AppRuntime;
            """;

        await AnalyzeWith<RuntimeMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DS0002")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*AppRuntime*partial*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsPartial()
    {
        const string source = """
            namespace TestApp;

            [Deepstaging.Runtime]
            public partial class AppRuntime;
            """;

        await AnalyzeWith<RuntimeMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenClassHasNoRuntimeAttribute()
    {
        const string source = """
            namespace TestApp;

            public class RegularClass;
            """;

        await AnalyzeWith<RuntimeMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
