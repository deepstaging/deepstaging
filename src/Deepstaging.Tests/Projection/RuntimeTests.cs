using Deepstaging.Projection;

namespace Deepstaging.Tests.Projection;

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

        [Deepstaging.EffectsModule(typeof(IEmailService))]
        [Deepstaging.EffectsModule(typeof(ISlackService))]
        public partial class EmailEffects;
        
        [Deepstaging.Runtime]
        [Deepstaging.Uses(typeof(EmailEffects))]
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
        await Assert.That(model.Capabilities.Length).IsEqualTo(2);
        await Assert.That(model.Capabilities[0].Interface).IsEqualTo("IHasEmailService");
        await Assert.That(model.Capabilities[1].Interface).IsEqualTo("IHasSlackService");
    }
}