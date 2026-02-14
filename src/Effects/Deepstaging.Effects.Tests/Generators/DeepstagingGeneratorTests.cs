// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Effects.Generators;

namespace Deepstaging.Effects.Tests.Generators;

public class DeepstagingGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task FullExample()
    {
        const string source =
            """
            using Deepstaging;
            using System.Threading.Tasks;

            namespace TestApp;

            public interface IGreeter
            {
                Task<string> SayHelloAsync(string name);
            }

            [EffectsModule(typeof(IGreeter))]
            public partial class GreeterModule;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .CompilesSuccessfully()
            .VerifySnapshot();
    }
}
