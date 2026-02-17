// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.CodeFixes.Effects;

public class ReplaceEffectsModuleWithCapabilityCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task ReplacesEffectsModuleWithCapability()
    {
        const string source =
            """
            namespace TestApp;

            public interface IConfigProvider
            {
                string ConnectionString { get; }
            }

            [Deepstaging.Effects.EffectsModule(typeof(IConfigProvider))]
            public sealed partial class ConfigEffects;
            """;

        const string expected =
            """
            namespace TestApp;

            public interface IConfigProvider
            {
                string ConnectionString { get; }
            }

            [Deepstaging.Effects.Capability(typeof(IConfigProvider))]
            public sealed partial class ConfigEffects;
            """;

        await AnalyzeAndFixWith<EffectsModuleTargetHasNoMethodsAnalyzer, ReplaceEffectsModuleWithCapabilityCodeFix>(source)
            .ForDiagnostic(EffectsModuleTargetHasNoMethodsAnalyzer.DiagnosticId)
            .ShouldProduce(expected);
    }
}
