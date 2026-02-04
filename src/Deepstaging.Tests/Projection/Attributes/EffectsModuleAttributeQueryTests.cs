using Deepstaging.Projection;

namespace Deepstaging.Tests.Projection.Attributes;

public class EffectsModuleAttributeQueryTests : RoslynTestBase
{
    [Test]
    public async Task Name_DefaultsToTargetTypeName_WhenNotSpecified()
    {
        var query = SymbolsFor("""
                               using Deepstaging;
                               using System.Threading.Tasks;

                               namespace TestApp;

                               public interface IEmailService;

                               [EffectsModule(typeof(IEmailService))]
                               public partial class EmailEffects;
                               """)
            .RequireNamedType("EmailEffects")
            .GetAttribute<EffectsModuleAttribute>()
            .Map(attr => attr.QueryEffectsModuleAttribute())
            .OrThrow("Expected EffectsModuleAttribute");

        await Assert.That(query.Name).IsEqualTo("EmailService");
    }

    [Test]
    public async Task Name_UsesCustomName_WhenSpecified()
    {
        var query = SymbolsFor("""
                               using Deepstaging;
                               using System.Threading.Tasks;

                               namespace TestApp;

                               public interface IEmailService;

                               [EffectsModule(typeof(IEmailService), Name = "Emails")]
                               public partial class EmailEffects;
                               """)
            .RequireNamedType("EmailEffects")
            .GetAttribute<EffectsModuleAttribute>()
            .Map(attr => attr.QueryEffectsModuleAttribute())
            .OrThrow("Expected EffectsModuleAttribute");
        
        await Assert.That(query.Name).IsEqualTo("Emails");
    }

    [Test]
    public async Task TargetType_Throws_When_TypeNotFound()
    {
        var query = SymbolsFor("""
                               using Deepstaging;
                               using System.Threading.Tasks;

                               namespace TestApp;

                               [EffectsModule(typeof(IEmailService))]
                               public partial class EmailEffects;
                               """)
            .RequireNamedType("EmailEffects")
            .GetAttribute<EffectsModuleAttribute>()
            .Map(attr => attr.QueryEffectsModuleAttribute())
            .OrThrow("Expected EffectsModuleAttribute");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
        {
            var _ = query.TargetType;
            return Task.CompletedTask;
        });
    }
}