using Deepstaging.Analyzers;
using Deepstaging.CodeFixes;

namespace Deepstaging.Tests.Analyzers;

public class EffectsModuleMustBePartialCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsPartialModifier()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public class EmailEffects;
            """;

        const string expected = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            """;

        await AnalyzeAndFixWith<EffectsModuleMustBePartialAnalyzer, EffectsModuleMustBePartialCodeFix>(source)
            .ForDiagnostic("DS0001")
            .ShouldProduce(expected);
    }
    
    [Test]
    public async Task AddsPartialModifier_WithAlternativeApi()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public class EmailEffects;
            """;

        const string expected = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            """;

        // Alternative API: chain WithAnalyzer
        await FixWith<EffectsModuleMustBePartialCodeFix>(source)
            .WithAnalyzer<EffectsModuleMustBePartialAnalyzer>()
            .ForDiagnostic("DS0001")
            .ShouldProduce(expected);
    }
}
