// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Effects.Testing.Generators;

namespace Deepstaging.Effects.Testing.Tests.Generators;

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

                  [Deepstaging.EffectsModule(typeof(IEmailService))]
                  [Deepstaging.EffectsModule(typeof(ISlackService))]
                  public partial class MyEffects;

                  [Deepstaging.Runtime]
                  [Deepstaging.Uses(typeof(MyEffects))]
                  public partial class AppRuntime;

                  [Deepstaging.TestRuntime<AppRuntime>]
                  public partial class TestAppRuntime;
                  """)
            .ShouldGenerate()
            .WithFileNamed("TestAppRuntime.g.cs")
            .VerifySnapshot();
    }
}
