using Deepstaging.Effects.Generators;
using Deepstaging.Generators;

namespace Deepstaging.Effects.Tests;

public class DispatcherModuleTests : RoslynTestBase
{
    [Test]
    public async Task StandaloneDispatcher_GeneratesCapabilityInterface()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestApp;
            
            public interface ICommand { }
            
            [DispatcherModule(CommandType = typeof(ICommand))]
            public partial class CommandDispatcher;
            """;

        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public interface IHasCommandDispatcher");
    }
    
    [Test]
    public async Task StandaloneDispatcher_WithHandlers_GeneratesDispatchMethods()
    {
        var source = """
            using Deepstaging.Effects;
            using System.Threading.Tasks;
            
            namespace TestApp;
            
            public interface ICommand { }
            public record CreateUserCommand(string GetModuleName) : ICommand;
            public record User(string Id, string GetModuleName);
            
            public static class CreateUserHandler
            {
                public static Task<User> Handle(CreateUserCommand cmd) 
                    => Task.FromResult(new User("1", cmd.GetModuleName));
            }
            
            [DispatcherModule(CommandType = typeof(ICommand))]
            public partial class CommandDispatcher;
            """;
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public static partial class CommandDispatcher")
            .WithFileContaining("Dispatch<RT>(global::TestApp.CreateUserCommand command)")
            .WithFileContaining("where RT : IHasCommandDispatcher");
    }
    
    [Test]
    public async Task StandaloneDispatcher_WithCustomName_UsesCustomName()
    {
        var source = """
            using Deepstaging.Effects;
            using System.Threading.Tasks;
            
            namespace TestApp;
            
            public interface ICommand { }
            public record DoSomethingCommand() : ICommand;
            
            public static class DoSomethingHandler
            {
                public static Task Handle(DoSomethingCommand cmd) => Task.CompletedTask;
            }
            
            [DispatcherModule(GetModuleName = "App", CommandType = typeof(ICommand))]
            public partial class MyDispatcher;
            """;
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("IHasAppDispatcher")
            .WithFileContaining("public static partial class AppDispatcher");
    }
    
    [Test]
    public async Task StandaloneDispatcher_WithEventQueue_AddsQueueConstraint()
    {
        var source = """
            using Deepstaging.Effects;
            using System.Collections.Generic;
            using System.Threading.Tasks;
            
            namespace TestApp;
            
            public interface ICommand { }
            public interface IDomainEvent { }
            public record UserCreated(string GetModuleName) : IDomainEvent;
            public record CreateUserCommand(string GetModuleName) : ICommand;
            
            public static class CreateUserHandler
            {
                public static Task<IEnumerable<IDomainEvent>> Handle(CreateUserCommand cmd) 
                    => Task.FromResult<IEnumerable<IDomainEvent>>(new[] { new UserCreated(cmd.GetModuleName) });
            }
            
            [DispatcherModule(CommandType = typeof(ICommand), EventQueueName = "DomainEvents")]
            public partial class CommandDispatcher;
            """;
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("IHasDomainEventsQueue")
            .WithFileContaining("DomainEventsWriter.WriteAsync");
    }
    
    [Test]
    public async Task StandaloneDispatcher_WithDbContext_AddsAutoCommit()
    {
        var source = """
            using Deepstaging.Effects;
            using System.Threading.Tasks;
            
            namespace TestApp;
            
            public interface ICommand { }
            public record CreateUserCommand(string GetModuleName) : ICommand;
            
            public static class CreateUserHandler
            {
                public static Task Handle(CreateUserCommand cmd) => Task.CompletedTask;
            }
            
            [DispatcherModule(CommandType = typeof(ICommand), DbContextModuleName = "Database")]
            public partial class CommandDispatcher;
            """;
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("IHasDatabase")
            .WithFileContaining("Database.SaveChangesAsync()");
    }
    
    [Test]
    public async Task Runtime_UsesDispatcherModule_AddsCapabilityInterface()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestApp;
            
            public interface ICommand { }
            
            [DispatcherModule(CommandType = typeof(ICommand))]
            public partial class CommandDispatcher;
            
            [Runtime]
            [Uses(typeof(CommandDispatcher))]
            public sealed partial class AppRuntime;
            """;
        
        await GenerateWith<DeepstagingGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("AppRuntime : IHasCommandDispatcher");
    }
}
