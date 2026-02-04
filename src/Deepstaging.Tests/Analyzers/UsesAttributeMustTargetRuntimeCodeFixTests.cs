using Deepstaging.Analyzers;
using Deepstaging.CodeFixes;

namespace Deepstaging.Tests.Analyzers;

public class UsesAttributeMustTargetRuntimeCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsRuntimeAttribute()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;

            [Deepstaging.Uses(typeof(EmailEffects))]
            public partial class AppRuntime;
            """;

        const string expected = """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;

            [Deepstaging.Runtime]
            [Deepstaging.Uses(typeof(EmailEffects))]
            public partial class AppRuntime;
            """;

        await AnalyzeAndFixWith<UsesAttributeMustTargetRuntimeAnalyzer, UsesAttributeMustTargetRuntimeCodeFix>(source)
            .ForDiagnostic("DS0003")
            .ShouldProduce(expected);
    }
}
