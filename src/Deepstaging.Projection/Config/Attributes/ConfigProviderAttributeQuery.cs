// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Config.Attributes;

/// <summary>
/// A queryable wrapper over <see cref="ConfigProviderAttribute"/> data.
/// Provides access to the section name — explicit or inferred from the class name.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record ConfigProviderAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    private const string Suffix = "ConfigProvider";

    /// <summary>
    /// Gets the configuration section name.
    /// Uses the explicit <c>Section</c> value when set; otherwise infers by stripping
    /// the <c>ConfigProvider</c> suffix from <paramref name="symbol"/>.
    /// Returns <c>null</c> when inference is not possible.
    /// </summary>
    /// <param name="symbol">The symbol of the class annotated with <see cref="ConfigProviderAttribute"/>.</param>
    public string GetSectionName(ValidSymbol<INamedTypeSymbol> symbol) =>
        NamedArg<string>(nameof(ConfigProviderAttribute.Section))
            .OrDefault(() => InferSection(symbol));

    private static string InferSection(ValidSymbol<INamedTypeSymbol> symbol) =>
        symbol.Map(type => type.Name.EndsWith(Suffix, StringComparison.Ordinal) && type.Name.Length > Suffix.Length
            ? type.Name[..^Suffix.Length]
            : string.Empty);
    
    /// <summary>
    /// 
    /// </summary>
    public string DataDirectory =>
        NamedArg<string>(nameof(ConfigProviderAttribute.DataDirectory))
            .OrDefault(".config");
}