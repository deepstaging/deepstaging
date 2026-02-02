using Deepstaging.Data;
using Deepstaging.Generators.Emitters;

namespace Deepstaging.Tests.Generators.Emitters;

public class EffectsModuleEmitterTests : RoslynTestBase
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

        [Deepstaging.EffectsModule(typeof(IEmailService))]
        public partial class EmailEffects;
        """;

    [Test]
    public async Task ProducesCapabilityInterface()
    {
        var emit = SymbolsFor(Source)
            .RequireNamedType("EmailEffects")
            .ReadEffectsModules()
            .First()
            .EmitCapabilityInterface();

        await Assert.That(emit).IsSuccessful();
        await Assert.That(emit).CodeContains("public interface IHasEmailService");
        await Assert.That(emit).CodeContains("IEmailService EmailService { get; }");
    }

    [Test]
    public async Task ProducesStaticEffectsModule()
    {
        var emit = SymbolsFor(Source)
            .RequireNamedType("EmailEffects")
            .ReadEffectsModules()
            .First()
            .EmitEffectsModule();
        
        await Assert.That(emit).IsSuccessful();
    }
}