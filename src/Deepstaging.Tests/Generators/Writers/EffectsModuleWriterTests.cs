using Deepstaging.Projection;
using Deepstaging.Generators.Writers;
using Deepstaging.Roslyn.Emit;

namespace Deepstaging.Tests.Generators.Writers;

public class EffectsModuleWriterTests : RoslynTestBase
{
    [Test]
    public async Task ProducesCapabilityInterface()
    {
        var emit = SymbolsFor("""
                              using System.Threading.Tasks;

                              namespace TestApp;
                              
                              public interface IEmailService
                              {
                                  Task SendAsync(string to, string subject, string body);
                                  Task<bool> ValidateAsync(string email);
                              }

                              [Deepstaging.EffectsModule(typeof(IEmailService))]
                              public partial class EmailEffects;
                              """)
            .RequireNamedType("EmailEffects")
            .QueryEffectsModules()
            .First()
            .WriteCapabilityInterface();

        await Assert.That(emit).IsSuccessful();

        var fullSource = $$"""
                           {{emit.Code!}}

                           public interface IEmailService
                           {
                               Task SendAsync(string to, string subject, string body);
                               Task<bool> ValidateAsync(string email);
                           }

                           [Deepstaging.EffectsModule(typeof(IEmailService))]
                           public partial class EmailEffects;
                           """;
        await Assert.That(CompilationFor(fullSource)).IsSuccessful();
        await Verify(emit.Code);
    }

    [Test]
    public async Task ProducesStaticEffectsModule()
    {
        var model = SymbolsFor("""
                               using System.Threading.Tasks;

                               namespace TestApp;
                               
                               /// <summary>
                               ///    Service for sending emails
                               /// </summary>
                               public interface IEmailService
                               {
                                   /// <summary>
                                   ///  Sends an email
                                   /// </summary>
                                   /// <param name="to">The recipient email address</param>
                                   /// <param name="subject">The email subject</param>
                                   /// <param name="body">The email body</param>
                                   Task SendAsync(string to, string subject, string body);
                                   Task<bool> ValidateAsync(string email);
                               }

                               [Deepstaging.EffectsModule(typeof(IEmailService))]
                               public partial class EmailEffects;
                               """)
            .RequireNamedType("EmailEffects")
            .QueryEffectsModules()
            .First();

        var effectsModule = model.WriteEffectsModule();
        var capabilityInterface = model.WriteCapabilityInterface();

        await Assert.That(effectsModule).IsSuccessful();

        var allGeneratedCode = OptionalEmit.Combine(effectsModule, capabilityInterface).ValidateOrThrow().Code;
        
        await Verify(allGeneratedCode);
        
        var fullSource = $$"""
                           {{allGeneratedCode}}

                           public interface IEmailService
                           {
                               Task SendAsync(string to, string subject, string body);
                               Task<bool> ValidateAsync(string email);
                           }

                           [Deepstaging.EffectsModule(typeof(IEmailService))]
                           public partial class EmailEffects;
                           """;

        await Assert.That(CompilationFor(fullSource)).IsSuccessful();

    }

    [Test]
    public async Task ProducesDbContextEffectsModule()
    {
        var source = """
                     using Microsoft.EntityFrameworkCore;

                     namespace TestApp;

                     public class User { public int Id { get; set; } }

                     public class AppDbContext : DbContext
                     {
                         public DbSet<User> Users { get; set; } = null!;
                     }

                     [Deepstaging.EffectsModule(typeof(AppDbContext), Name = "Database")]
                     public partial class MyEffects;
                     """;

        var model = SymbolsFor(source)
            .RequireNamedType("MyEffects")
            .QueryEffectsModules()
            .First();

        var effectsModule = model.WriteEffectsModule();
        var capabilityInterface = model.WriteCapabilityInterface();
        
        await Assert.That(effectsModule).IsSuccessful();
        await Verify(effectsModule.Code);

        var fullSource = $$"""
                           {{OptionalEmit.Combine(effectsModule, capabilityInterface).ValidateOrThrow().Code}}

                           public class User { public int Id { get; set; } }

                           public class AppDbContext : DbContext
                           {
                               public DbSet<User> Users { get; set; } = null!;
                           }

                           [Deepstaging.EffectsModule(typeof(AppDbContext), Name = "Database")]
                           public partial class MyEffects;
                           """;

        await Assert.That(CompilationFor(fullSource)).IsSuccessful();
    }
}