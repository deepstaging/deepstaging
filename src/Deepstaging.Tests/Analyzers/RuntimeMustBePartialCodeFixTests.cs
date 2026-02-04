using Deepstaging.Analyzers;
using Deepstaging.CodeFixes;

namespace Deepstaging.Tests.Analyzers;

public class RuntimeMustBePartialCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsPartialModifier()
    {
        const string source = """
            namespace TestApp;

            [Deepstaging.Runtime]
            public class AppRuntime;
            """;

        const string expected = """
            namespace TestApp;

            [Deepstaging.Runtime]
            public partial class AppRuntime;
            """;

        await AnalyzeAndFixWith<RuntimeMustBePartialAnalyzer, RuntimeMustBePartialCodeFix>(source)
            .ForDiagnostic("DS0002")
            .ShouldProduce(expected);
    }
}
