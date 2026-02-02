using Deepstaging.Generators;

namespace Deepstaging.Effects.Tests;

public class EffectsModuleTests : RoslynTestBase
{
    [Test]
    public async Task StandaloneModule_GeneratesCapabilityAndEffects()
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
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public interface IHasEmail")
            .WithFileContaining("IEmailService EmailService { get; }")
            .WithFileContaining("public static partial class EmailEffects")
            .WithFileContaining("Eff<RT, Unit> SendAsync<RT>")
            .WithFileContaining("Eff<RT, bool> ValidateAsync<RT>")
            .WithFileContaining("where RT : IHasEmail")
            .VerifySnapshot();
    }
    
    [Test]
    public async Task StandaloneModule_WithCustomName_UsesCustomName()
    {
        var source = """
            using Deepstaging.Effects;
            using System.Threading.Tasks;
            
            namespace TestApp;
            
            public interface IEmailService
            {
                Task SendAsync(string to, string body);
            }
            
            [EffectsModule(typeof(IEmailService), GetModuleName = "Mailer")]
            public partial class MailerModule;
            """;
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("IHasMailer")
            .WithFileContaining("public static partial class MailerEffects")
            .VerifySnapshot();
    }
    
    [Test]
    public async Task StandaloneModule_WithExclude_FiltersOutMethods()
    {
        var source = """
            using Deepstaging.Effects;
            using System.Threading.Tasks;
            
            namespace TestApp;
            
            /// <summary>
            /// Email service interface.
            /// </summary>
            public interface IEmailService
            {
                /// <summary>
                /// Sends an email asynchronously.
                /// </summary>
                /// <param name="to"></param>
                /// <param name="body"></param>
                /// <returns></returns>
                Task SendAsync(string to, string body);
                
                /// <summary>
                /// Validates an email address asynchronously.
                /// </summary>
                /// <param name="email"></param>
                /// <returns></returns>
                Task<bool> ValidateAsync(string email);
                
                /// <summary>
                /// Disposes the email service.
                /// </summary>
                void Dispose();
            }
            
            [EffectsModule(typeof(IEmailService), Exclude = ["Dispose"])]
            public partial class EmailEffects;
            """;
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("SendAsync")
            .WithFileContaining("ValidateAsync")
            .WithoutFileContaining("Dispose<RT>");
    }
    
    [Test]
    public async Task StandaloneModule_WithIncludeOnly_OnlyIncludesSpecifiedMethods()
    {
        var source = """
            using Deepstaging.Effects;
            using System.Threading.Tasks;
            
            namespace TestApp;
            
            public interface IEmailService
            {
                Task SendAsync(string to, string body);
                Task<bool> ValidateAsync(string email);
                Task<string> FormatAsync(string template);
            }
            
            [EffectsModule(typeof(IEmailService), IncludeOnly = ["SendAsync"])]
            public partial class EmailEffects;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("SendAsync<RT>")
            .WithoutFileContaining("ValidateAsync<RT>")
            .WithoutFileContaining("FormatAsync<RT>");
    }
    
    [Test]
    public async Task StandaloneModule_WithInstrumentedFalse_OmitsActivitySource()
    {
        var source = """
            using Deepstaging.Effects;
            using System.Threading.Tasks;
            
            namespace TestApp;
            
            public interface IEmailService
            {
                Task SendAsync(string to, string body);
            }
            
            [EffectsModule(typeof(IEmailService), Instrumented = false)]
            public partial class EmailEffects;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("SendAsync")
            .WithoutFileContaining("ActivitySource")
            .WithoutFileContaining("WithActivity");
    }
    
    [Test]
    public async Task StandaloneModule_DbContext_GeneratesEntityGroups()
    {
        var source = """
            using Deepstaging.Effects;
            using Microsoft.EntityFrameworkCore;
            
            namespace TestApp;
            
            public class User { public int Id { get; set; } }
            public class Order { public int Id { get; set; } }
            
            public class AppDbContext : DbContext
            {
                public DbSet<User> Users { get; set; } = null!;
                public DbSet<Order> Orders { get; set; } = null!;
            }
            
            [EffectsModule(typeof(AppDbContext), GetModuleName = "Database")]
            public partial class DatabaseEffects;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("IHasDatabase")
            .WithFileContaining("public static partial class DatabaseEffects")
            .WithFileContaining("public static partial class Users")
            .WithFileContaining("public static partial class Orders")
            .WithFileContaining("SaveChangesAsync")
            .WithFileContaining("FindAsync")
            .WithFileContaining("DbSetQuery<RT, global::TestApp.User>")
            .VerifySnapshot();
    }
    
    [Test]
    public async Task StandaloneModule_DerivesNameFromTypeName()
    {
        var source = """
            using Deepstaging.Effects;
            using System.Threading.Tasks;
            
            namespace TestApp;
            
            // INotificationService -> "Notification"
            public interface INotificationService
            {
                Task NotifyAsync(string message);
            }
            
            [EffectsModule(typeof(INotificationService))]
            public partial class NotificationModule;
            """;
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("IHasNotification")
            .WithFileContaining("NotificationEffects");
    }

    [Test]
    public async Task Runtime_UsesStandaloneModule_InjectsDependency()
    {
        // This test verifies that a runtime using [Uses] to reference a standalone module
        // gets the dependency automatically injected
        var source = """
                     using Deepstaging.Effects;
                     using System.Threading.Tasks;

                     namespace TestApp;

                     public interface IEmailService
                     {
                         Task SendAsync(string to, string body);
                     }

                     // Standalone module generates IHasEmail + EmailEffects
                     [EffectsModule(typeof(IEmailService))]
                     public partial class EmailModule;

                     // Runtime uses the standalone module via [Uses] attribute
                     [Runtime]
                     [Uses(typeof(EmailModule))]
                     public sealed partial class AppRuntime;
                     """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("IEmailService emailService")
            .WithFileContaining("public global::TestApp.IEmailService EmailService { get; }")
            .WithFileContaining("AppRuntime : IHasEmail")
            .WithFileContaining("public static partial class EmailEffects")
            .WithFileContaining("public interface IHasEmail");
    }
}
