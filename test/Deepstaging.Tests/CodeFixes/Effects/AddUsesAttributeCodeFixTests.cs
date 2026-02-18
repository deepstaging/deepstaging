// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.CodeFixes.Effects;

public class AddUsesAttributeCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task AddsUsesAttribute_ForUnusedModule()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            [Deepstaging.Effects.Runtime]
            public partial class AppRuntime;
            """;

        const string expected =
            """
            namespace TestApp;

            public interface IEmailService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            [Uses(typeof(EmailModule))]
            [Deepstaging.Effects.Runtime]
            public partial class AppRuntime;
            """;

        await AnalyzeAndFixWith<AvailableEffectsModuleAnalyzer, AddUsesAttributeCodeFix>(source)
            .ForDiagnostic(AvailableEffectsModuleAnalyzer.DiagnosticId)
            .ShouldProduce(expected);
    }

    [Test]
    public async Task PreservesExistingUsesAttributes()
    {
        const string source =
            """
            namespace TestApp;

            public interface IEmailService { }
            public interface ISlackService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            [Deepstaging.Effects.EffectsModule(typeof(ISlackService))]
            public sealed partial class SlackModule;

            [Deepstaging.Effects.Runtime]
            [Deepstaging.Effects.Uses(typeof(EmailModule))]
            public partial class AppRuntime;
            """;

        const string expected =
            """
            namespace TestApp;

            public interface IEmailService { }
            public interface ISlackService { }

            [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailModule;

            [Deepstaging.Effects.EffectsModule(typeof(ISlackService))]
            public sealed partial class SlackModule;

            [Uses(typeof(SlackModule))]
            [Deepstaging.Effects.Runtime]
            [Deepstaging.Effects.Uses(typeof(EmailModule))]
            public partial class AppRuntime;
            """;

        await AnalyzeAndFixWith<AvailableEffectsModuleAnalyzer, AddUsesAttributeCodeFix>(source)
            .ForDiagnostic(AvailableEffectsModuleAnalyzer.DiagnosticId)
            .ShouldProduce(expected);
    }
}
