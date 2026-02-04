namespace Deepstaging;

/// <summary>
/// Marks a partial class as a Deepstaging Effects runtime.
/// The runtime aggregates all modules referenced via <see cref="UsesAttribute"/> and generates 
/// constructor injection, properties, and DI registration.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RuntimeAttribute : Attribute;
