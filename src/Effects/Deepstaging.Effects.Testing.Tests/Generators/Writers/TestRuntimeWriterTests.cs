// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Testing.Tests.Generators.Writers;

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

                [Deepstaging.EffectsModule(typeof(IEmailService))]
                [Deepstaging.EffectsModule(typeof(ISlackService))]
                public partial class MyEffects;

                [Deepstaging.Runtime]
                [Deepstaging.Uses(typeof(MyEffects))]
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

                [Deepstaging.EffectsModule(typeof(IEmailService))]
                public partial class EmailModule;

                [Deepstaging.Runtime]
                [Deepstaging.Uses(typeof(EmailModule))]
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
