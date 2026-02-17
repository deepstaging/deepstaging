// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.CodeFixes.Effects;

public class ClassMustBePartialCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task EffectsModule_AddsPartialModifier()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public class EmailEffects;
            """;

        const string expected =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            """;

        await AnalyzeAndFixWith<EffectsModuleMustBePartialAnalyzer, ClassMustBePartialCodeFix>(source)
            .ForDiagnostic(EffectsModuleMustBePartialAnalyzer.DiagnosticId)
            .ShouldProduce(expected);
    }

    [Test]
    public async Task Runtime_AddsPartialModifier()
    {
        const string source =
            """
            namespace TestApp;

            [Deepstaging.Effects.Runtime]
            public class AppRuntime;
            """;

        const string expected =
            """
            namespace TestApp;

            [Deepstaging.Effects.Runtime]
            public partial class AppRuntime;
            """;

        await AnalyzeAndFixWith<RuntimeMustBePartialAnalyzer, ClassMustBePartialCodeFix>(source)
            .ForDiagnostic(RuntimeMustBePartialAnalyzer.DiagnosticId)
            .ShouldProduce(expected);
    }
}
