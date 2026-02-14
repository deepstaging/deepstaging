// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Tests.CodeFixes;

public class UsesAttributeMustTargetRuntimeCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsRuntimeAttribute()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;

            [Deepstaging.Uses(typeof(EmailEffects))]
            public partial class AppRuntime;
            """;

        const string expected =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;

            [Deepstaging.Runtime]
            [Deepstaging.Uses(typeof(EmailEffects))]
            public partial class AppRuntime;
            """;

        await AnalyzeAndFixWith<UsesAttributeMustTargetRuntimeAnalyzer, UsesAttributeMustTargetRuntimeCodeFix>(source)
            .ForDiagnostic(UsesAttributeMustTargetRuntimeAnalyzer.DiagnosticId)
            .ShouldProduce(expected);
    }
}
