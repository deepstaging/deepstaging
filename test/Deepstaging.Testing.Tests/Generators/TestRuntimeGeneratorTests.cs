// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Testing.Generators;

namespace Deepstaging.Testing.Tests.Generators;

public class TestRuntimeGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task GeneratesTestRuntime()
    {
        await GenerateWith<TestRuntimeGenerator>(
                $$"""
                  using System.Threading.Tasks;
                  namespace TestApp;

                  public interface IEmailService
                  {
                      Task SendAsync(string to, string subject, string body);
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
            .ShouldGenerate()
            .WithFileNamed("TestAppRuntime.g.cs")
            .VerifySnapshot();
    }
}
