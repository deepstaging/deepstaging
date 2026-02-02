using Deepstaging.Data;
using Deepstaging.Generators;
using Deepstaging.Generators.Emitters;

namespace Deepstaging.Tests.Generators.Emitters;

public class EffectsModuleEmitterTests : RoslynTestBase
{
    public async Task ProducesCapabilityInterface_ForEffectsModule()
    {
        var source = """
            using Deepstaging.Effects;
            using System.Threading.Tasks;
            
            namespace TestApp;
            
            public interface IEmailService
            {
                Task SendAsync(string to, string subject, string body);
                Task<bool> ValidateAsync(string email);
            }
            
            [EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            """;
        
        var emit = SymbolsFor(source)
            .RequireNamedType("EmailEffects")
            .ReadEffectsModules()
            .First()
            .EmitCapabilityInterface();
        
        Assert.That(emit).Is
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public interface IHasEmailService")
            .WithFileContaining("IEmailService EmailService { get; }")
            .VerifySnapshot();
    }
}

