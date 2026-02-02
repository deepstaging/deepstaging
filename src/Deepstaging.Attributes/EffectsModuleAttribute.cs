namespace Deepstaging;

/// <summary>
/// Defines a standalone effects module that wraps methods from a target type into LanguageExt effects.
/// </summary>
/// <remarks>
/// <para>
/// An effects module generates effect wrappers for all public methods on the target type,
/// lifting them into the <c>Eff&lt;RT, T&gt;</c> monad for functional composition.
/// </para>
/// <para>
/// The generated code includes:
/// <list type="bullet">
///   <item>A capability interface <c>IHas{Name}</c> with a property for the target type</item>
///   <item>A static class <c>{Name}Effects</c> with generic effect methods</item>
///   <item>Optional OpenTelemetry instrumentation via <c>.WithActivity()</c></item>
/// </list>
/// </para>
/// <para>
/// Method lifting strategies:
/// <list type="bullet">
///   <item><c>void</c> → <c>Eff&lt;RT, Unit&gt;</c></item>
///   <item><c>T</c> → <c>Eff&lt;RT, T&gt;</c></item>
///   <item><c>Task</c> → <c>Eff&lt;RT, Unit&gt;</c> (async)</item>
///   <item><c>Task&lt;T&gt;</c> → <c>Eff&lt;RT, T&gt;</c> (async)</item>
///   <item><c>Task&lt;T?&gt;</c> → <c>Eff&lt;RT, Option&lt;T&gt;&gt;</c> (nullable to Option)</item>
/// </list>
/// </para>
/// <para>
/// For <c>DbContext</c> types, the generator produces entity-specific query builders
/// with methods like <c>FindAsync</c>, <c>Add</c>, <c>Remove</c>, and <c>SaveChangesAsync</c>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Define an effects module for a service interface
/// [EffectsModule(typeof(IEmailService))]
/// public partial class EmailModule;
/// 
/// // Use in a runtime
/// [Runtime]
/// [Uses(typeof(EmailModule))]
/// public sealed partial class AppRuntime;
/// 
/// // Generated effect usage:
/// var sendEffect = EmailEffects.SendAsync&lt;AppRuntime&gt;("user@example.com", "Hello!");
/// </code>
/// </example>
/// <example>
/// <code>
/// // Effects module for a DbContext
/// [EffectsModule(typeof(AppDbContext), Name = "Database")]
/// public partial class DatabaseModule;
/// 
/// // Generated:
/// // DatabaseEffects.Users.FindAsync&lt;AppRuntime&gt;(id)
/// // DatabaseEffects.Users.Add&lt;AppRuntime&gt;(user)
/// // DatabaseEffects.SaveChangesAsync&lt;AppRuntime&gt;()
/// </code>
/// </example>
/// <seealso cref="UsesAttribute"/>
/// <seealso cref="RuntimeAttribute"/>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EffectsModuleAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of the module.
    /// If not set, the name is derived from the target type (e.g., "EmailService" for IEmailService).
    /// </summary>
    public string? Name { get; init; } 
    
    /// <summary>
    /// Gets the target type whose methods will be wrapped as effects.
    /// Can be an interface (methods wrapped) or a <c>DbContext</c> (entity query builders generated).
    /// </summary>
    public Type TargetType { get; }
    
    /// <summary>
    /// Gets or sets whether OpenTelemetry instrumentation is enabled.
    /// When <c>true</c> (default), generated effects include <c>.WithActivity()</c> calls
    /// that create spans for tracing. Zero overhead when no <c>ActivityListener</c> is registered.
    /// </summary>
    public bool Instrumented { get; init; } = true;
    
    /// <summary>
    /// When set, ONLY these methods are wrapped as effects.
    /// All other methods on the target type are ignored.
    /// Takes precedence over <see cref="Exclude"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // Only wrap SendAsync and SendBulkAsync
    /// [EffectsModule(typeof(IEmailService), IncludeOnly = ["SendAsync", "SendBulkAsync"])]
    /// public partial class EmailModule;
    /// </code>
    /// </example>
    public string[]? IncludeOnly { get; init; }
    
    /// <summary>
    /// Methods to exclude from effect generation.
    /// Ignored if <see cref="IncludeOnly"/> is set.
    /// </summary>
    /// <example>
    /// <code>
    /// // Exclude internal/diagnostic methods
    /// [EffectsModule(typeof(IEmailService), Exclude = ["GetStatistics", "Ping"])]
    /// public partial class EmailModule;
    /// </code>
    /// </example>
    public string[]? Exclude { get; init; }
    
    /// <summary>
    /// Creates a new EffectsModule attribute.
    /// </summary>
    /// <param name="targetType">The type to wrap. Can be an interface or <c>DbContext</c>.</param>
    public EffectsModuleAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}
