// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Testing.Tests.Generators.Writers;

public class TestRuntimeWriterTests : RoslynTestBase
{
    [Test]
    public async Task ProducesTestRuntimeClass()
    {
        var model = SymbolsFor(
                """
                using System.Threading.Tasks;
                namespace TestApp;

                public interface IEmailService
                {
                    Task<string> SendAsync(string to, string subject, string body);
                }

                public interface ISlackService
                {
                    Task PostMessageAsync(string channel, string message);
                }

                [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
                [Deepstaging.Effects.EffectsModule(typeof(ISlackService))]
                public partial class MyEffects;

                [Deepstaging.Effects.Runtime]
                [Deepstaging.Effects.Uses(typeof(MyEffects))]
                public partial class AppRuntime;

                [Deepstaging.TestRuntime<AppRuntime>]
                public partial class TestAppRuntime;
                """)
            .RequireNamedType("TestAppRuntime")
            .QueryTestRuntimeModel();

        var testRuntime = model.WriteTestRuntimeClass();

        await Assert.That(testRuntime).IsSuccessful();
        await Verify(testRuntime.Code);
    }

    [Test]
    public async Task ProducesTestRuntimeWithSingleCapability()
    {
        var model = SymbolsFor(
                """
                using System.Threading.Tasks;
                namespace TestApp;

                public interface IEmailService
                {
                    Task SendAsync(string to, string subject, string body);
                }

                [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
                public partial class EmailModule;

                [Deepstaging.Effects.Runtime]
                [Deepstaging.Effects.Uses(typeof(EmailModule))]
                public partial class AppRuntime;

                [Deepstaging.TestRuntime<AppRuntime>]
                public partial class TestAppRuntime;
                """)
            .RequireNamedType("TestAppRuntime")
            .QueryTestRuntimeModel();

        var testRuntime = model.WriteTestRuntimeClass();

        await Assert.That(testRuntime).IsSuccessful();
        await Verify(testRuntime.Code);
    }
}
