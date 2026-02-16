// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Generators;

namespace Deepstaging.Tests.Generators.Effects;

public class DeepstagingGeneratorTests : RoslynTestBase
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

            [EffectsModule(typeof(IGreeter))]
            public partial class GreeterModule;
            """;

        await GenerateWith<EffectsGenerator>(source)
            .ShouldGenerate()
            .CompilesSuccessfully()
            .VerifySnapshot();
    }
}
