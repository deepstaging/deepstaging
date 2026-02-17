// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.CodeFixes.Effects;

public class ClassShouldBeSealedCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsSealedModifier()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            """;

        const string expected =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeAndFixWith<EffectsModuleShouldBeSealedAnalyzer, ClassShouldBeSealedCodeFix>(source)
            .ForDiagnostic(EffectsModuleShouldBeSealedAnalyzer.DiagnosticId)
            .ShouldProduce(expected);
    }
}
