// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Projection;
using Deepstaging.Generators.Writers;

namespace Deepstaging.Tests.Generators.Writers;

public class RuntimeWriterTests : RoslynTestBase
{
    private static readonly string Source =
        """
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
        public partial class Runtime;
        """;

    [Test]
    public async Task ProducesRuntimeClass()
    {
        var model = SymbolsFor(
                $"""
                 using System.Threading.Tasks;
                 namespace TestApp;
                 {Source}
                 """)
            .RequireNamedType("Runtime")
            .QueryRuntimeModel();
        
        var runtime = model.WriteRuntimeClass();

        await Assert.That(runtime).IsSuccessful();

        await Verify(runtime.Code);
    }

    [Test]
    public async Task ProducesBootstrapperClass()
    {
        var model = SymbolsFor(
                $"""
                 using System.Threading.Tasks;
                 namespace TestApp;
                 {Source}
                 """)
            .RequireNamedType("Runtime")
            .QueryRuntimeModel();
        
        var bootstrapper = model.WriteRuntimeBootstrapperClass();
        await Assert.That(bootstrapper).IsSuccessful();
        await Verify(bootstrapper.Code);

        var fullSource =
            $$"""
              {{bootstrapper.Code!}}               
              {{Source}}
              """;
        
        await Assert.That(CompilationFor(fullSource)).IsSuccessful();

    }
}