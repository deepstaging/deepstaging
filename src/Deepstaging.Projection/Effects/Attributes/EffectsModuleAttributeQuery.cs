// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects.Attributes;

using Attr = EffectsModuleAttribute;

/// <summary>
/// A queryable wrapper over <see cref="EffectsModuleAttribute"/> data.
/// Provides strongly-typed access to attribute properties with sensible defaults.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record EffectsModuleAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// Gets the module name. Defaults to the target type name (with leading 'I' stripped for interfaces).
    /// </summary>
    /// <example>
    /// <code>
    /// // Explicit name
    /// [EffectsModule(typeof(IEmailService), Name = "Mail")]
    /// 
    /// // Derived name: "EmailService" (from IEmailService)
    /// [EffectsModule(typeof(IEmailService))]
    /// </code>
    /// </example>
    public string Name => NamedArg<string>(nameof(Attr.Name))
        .OrDefault(() => TargetType switch
            {
                { IsInterface: true, Name: { Length: > 1 } name } when name[0] == 'I' => name.Substring(1),
                _ => TargetType.Name
            }
        );

    /// <summary>
    /// Gets the target type whose methods will be wrapped as effects.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the target type is not valid.</exception>
    public ValidSymbol<INamedTypeSymbol> TargetType => ConstructorArg<INamedTypeSymbol>(0)
        .Map(symbol => symbol.AsValidNamedType())
        .OrThrow($"{nameof(Attr)} must have a valid target type as its first constructor argument.");


    /// <summary>
    /// Gets whether OpenTelemetry instrumentation is enabled. Defaults to <c>true</c>.
    /// </summary>
    public bool Instrumented => NamedArg<bool>(nameof(Attr.Instrumented))
        .OrDefault(true);

    /// <summary>
    /// Gets the list of method names to include exclusively. Empty means include all (subject to <see cref="Exclude"/>).
    /// </summary>
    public ImmutableArray<string> IncludeOnly => NamedArg<string[]>(nameof(Attr.IncludeOnly))
        .Map(ImmutableArray.Create)
        .OrDefault([]);

    /// <summary>
    /// Gets the list of method names to exclude from effect generation.
    /// Ignored if <see cref="IncludeOnly"/> is set.
    /// </summary>
    public ImmutableArray<string> Exclude => NamedArg<string[]>(nameof(Attr.Exclude))
        .Map(ImmutableArray.Create)
        .OrDefault([]);
}