using Deepstaging.Analyzers;
using Deepstaging.CodeFixes;

namespace Deepstaging.Tests.Analyzers;

public class EffectsModuleShouldBeSealedCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsSealedModifier()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            """;

        const string expected = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeAndFixWith<EffectsModuleShouldBeSealedAnalyzer, EffectsModuleShouldBeSealedCodeFix>(source)
            .ForDiagnostic("DS0009")
            .ShouldProduce(expected);
    }
}
