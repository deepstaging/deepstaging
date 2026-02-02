using Deepstaging.Effects.Generators;
using Deepstaging.Generators;

namespace Deepstaging.Effects.Tests;

public class EventQueueModuleTests : RoslynTestBase
{
    [Test]
    public async Task StandaloneEventQueue_GeneratesCapabilityAndEffects()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestApp;
            
            public interface IDomainEvent { }
            
            [EventQueueModule(typeof(IDomainEvent))]
            public partial class DomainEventsQueue;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public interface IHasDomainEventsQueue")
            .WithFileContaining("ChannelWriter<global::TestApp.IDomainEvent> DomainEventsWriter { get; }")
            .WithFileContaining("public static partial class DomainEventsQueueEffects")
            .WithFileContaining("Eff<RT, Unit> Publish<RT>")
            .WithFileContaining("where RT : IHasDomainEventsQueue")
            .WithFileContaining("public sealed class DomainEventsQueueService : BackgroundService");
    }
    
    [Test]
    public async Task StandaloneEventQueue_WithCustomName_UsesCustomName()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestApp;
            
            public interface IEvent { }
            
            [EventQueueModule(typeof(IEvent), GetModuleName = "AppEvents")]
            public partial class MyQueue;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public interface IHasAppEventsQueue")
            .WithFileContaining("public static partial class AppEventsQueueEffects")
            .WithFileContaining("public sealed class AppEventsQueueService : BackgroundService");
    }
    
    [Test]
    public async Task StandaloneEventQueue_WithConcurrency_GeneratesSemaphore()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestApp;
            
            public interface IDomainEvent { }
            
            [EventQueueModule(typeof(IDomainEvent), MaxConcurrency = 5)]
            public partial class DomainEventsQueue;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("new SemaphoreSlim(5)")
            .WithFileContaining("Process up to 5 events concurrently");
    }

    [Test]
    public async Task Runtime_UsesEventQueueModule_InjectsChannelWriter()
    {
        var source = """
                     using Deepstaging.Effects;

                     namespace TestApp;

                     public interface IDomainEvent { }

                     [EventQueueModule(typeof(IDomainEvent))]
                     public partial class DomainEventsQueue;

                     [Runtime]
                     [Uses(typeof(DomainEventsQueue))]
                     public sealed partial class AppRuntime;
                     """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("ChannelWriter<global::TestApp.IDomainEvent> domainEventsWriter")
            .WithFileContaining("DomainEventsWriter { get; }")
            .WithFileContaining("AppRuntime : IHasDomainEventsQueue")
            .VerifySnapshot();
    }
}
