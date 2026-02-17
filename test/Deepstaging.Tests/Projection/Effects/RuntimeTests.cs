// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Projection.Effects;

public class RuntimeTests : RoslynTestBase
{
    private static readonly string Source =
        """
        using System.Threading.Tasks;

        namespace TestApp;

        public interface IEmailService
        {
            Task SendAsync(string to, string subject, string body);
            Task<bool> ValidateAsync(string email);
        }

        public interface ISlackService
        {
            Task PostMessageAsync(string channel, string message);
        }

        [Deepstaging.Effects.EffectsModule(typeof(IEmailService))]
        [Deepstaging.Effects.EffectsModule(typeof(ISlackService))]
        public partial class EmailEffects;

        [Deepstaging.Effects.Runtime]
        [Deepstaging.Effects.Uses(typeof(EmailEffects))]
        public partial class Runtime;
        """;

    [Test]
    public async Task QueriesRuntimeModel()
    {
        var model = SymbolsFor(Source)
            .RequireNamedType("Runtime")
            .QueryRuntimeModel();

        await Assert.That(model.RuntimeType).IsEqualTo("TestApp.Runtime");
        await Assert.That(model.RuntimeTypeName).IsEqualTo("Runtime");
        await Assert.That(model.Namespace).IsEqualTo("TestApp");
        await Assert.That(model.AccessibilityModifier).IsEqualTo("public");
        await Assert.That(model.Capabilities.Count).IsEqualTo(2);
        await Assert.That(model.Capabilities[0].Interface).IsEqualTo("IHasEmailService");
        await Assert.That(model.Capabilities[1].Interface).IsEqualTo("IHasSlackService");
    }
}
