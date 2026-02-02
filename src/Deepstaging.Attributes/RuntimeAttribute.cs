namespace Deepstaging;

/// <summary>
/// Marks a partial class as a Deepstaging Effects runtime.
/// The runtime aggregates all modules referenced via <see cref="UsesAttribute"/> and generates 
/// constructor injection, properties, and DI registration.
/// </summary>
/// <remarks>
/// <para>
/// A runtime is the central composition point that brings together standalone modules.
/// Each <c>[Uses(typeof(Module))]</c> attribute adds a module's dependencies and capability interfaces.
/// </para>
/// <para>
/// The generator produces:
/// <list type="bullet">
///   <item>A constructor accepting all aggregated dependencies</item>
///   <item>Properties for each dependency</item>
///   <item>Implementation of capability interfaces from all used modules</item>
///   <item>An <c>Add{RuntimeName}()</c> extension method for DI registration</item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Define standalone modules
/// [EffectsModule(typeof(IEmailService))]
/// public partial class EmailModule;
/// 
/// [EventQueueModule(typeof(IDomainEvent))]
/// public partial class DomainEventsQueue;
/// 
/// // Compose into a runtime
/// [Runtime]
/// [Uses(typeof(EmailModule))]
/// [Uses(typeof(DomainEventsQueue))]
/// public sealed partial class AppRuntime;
/// 
/// // Register in DI
/// services.AddAppRuntime();
/// </code>
/// </example>
/// <seealso cref="UsesAttribute"/>
/// <seealso cref="EffectsModuleAttribute"/>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RuntimeAttribute : Attribute;
