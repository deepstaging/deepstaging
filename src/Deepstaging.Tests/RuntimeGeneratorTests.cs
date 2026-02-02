using Deepstaging.Generators;

namespace Deepstaging.Tests;

public class RuntimeGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task EmptyRuntime_GeneratesConstructor()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestApp;
            
            [Runtime]
            public sealed partial class AppRuntime;
            """;
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public AppRuntime()");
    }
    
    [Test]
    public async Task EmptyRuntime_GeneratesDIExtension()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestApp;
            
            [Runtime]
            public sealed partial class AppRuntime;
            """;
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("AddAppRuntime");
    }
    
    [Test]
    public async Task Runtime_WithUsesEffectsModule_GeneratesDependency()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestApp;
            
            public interface IEmailService
            {
                void Send(string to, string body);
            }
            
            [EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            
            [Runtime]
            [Uses(typeof(EmailEffects))]
            public sealed partial class AppRuntime;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("IEmailService emailService")
            .WithFileContaining("EmailService { get; }");
    }
    
    [Test]
    public async Task Runtime_WithMultipleUsesModules_AggregatesDependencies()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestApp;
            
            public interface IEmailService { }
            public interface INotificationService { }
            
            [EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            
            [EffectsModule(typeof(INotificationService))]
            public partial class NotificationEffects;
            
            [Runtime]
            [Uses(typeof(EmailEffects))]
            [Uses(typeof(NotificationEffects))]
            public sealed partial class AppRuntime;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("IEmailService emailService")
            .WithFileContaining("INotificationService notificationService")
            .WithFileContaining("EmailService { get; }")
            .WithFileContaining("NotificationService { get; }");
    }
    
    [Test]
    public async Task Runtime_ImplementsCapabilityInterfaces()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestApp;
            
            public interface IEmailService { }
            
            [EffectsModule(typeof(IEmailService))]
            public partial class EmailEffects;
            
            [Runtime]
            [Uses(typeof(EmailEffects))]
            public sealed partial class AppRuntime;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("AppRuntime : IHasEmail");
    }
}
