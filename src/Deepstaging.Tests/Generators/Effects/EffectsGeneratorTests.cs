// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Generators;

namespace Deepstaging.Tests.Generators.Effects;

public class EffectsGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task FullExample()
    {
        const string source =
            """
            using Deepstaging.Effects;
            using System.Threading.Tasks;

            namespace TestApp;

            public interface IGreeter
            {
                Task<string> SayHelloAsync(string name);
            }
            
            public interface INumberProvider
            {
                Task<int> GetNumberAsync();
            }
            
            public interface IConfigProvider
            {
                string MyProperty { get; }
            }

            [EffectsModule(typeof(IGreeter))]
            [EffectsModule(typeof(INumberProvider))]
            [Capability(typeof(IConfigProvider))]
            public partial class GreeterModule;
            """;

        await GenerateWith<EffectsGenerator>(source)
            .ShouldGenerate()
            .CompilesSuccessfully()
            .VerifySnapshot();
    }
}
