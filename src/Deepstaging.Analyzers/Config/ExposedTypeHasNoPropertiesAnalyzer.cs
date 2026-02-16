// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Config;

using Deepstaging.Config;
using Deepstaging.Projection.Config;

/// <summary>
/// Reports a diagnostic when a type referenced by <see cref="ExposesAttribute{T}"/> has no public instance properties.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "Exposed configuration type has no properties",
    Message = "Type '{0}' exposed by [Exposes<T>] on '{1}' has no public instance properties",
    Description =
        "Configuration types exposed via [Exposes<T>] should have at least one public instance property. " +
        "A type with no properties produces an empty configuration section.",
    Severity = DiagnosticSeverity.Warning
)]
public sealed class ExposedTypeHasNoPropertiesAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for exposed types with no public properties.
    /// </summary>
    public const string DiagnosticId = "CFG004";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<ConfigProviderAttribute>() && GetFirstEmptyExposedType(type) is not null;

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type) =>
        [GetFirstEmptyExposedType(type)!.Value.Name, type.Name];

    private static ValidSymbol<INamedTypeSymbol>? GetFirstEmptyExposedType(ValidSymbol<INamedTypeSymbol> type)
    {
        foreach (var attr in type.ExposesAttributes())
        {
            var configType = attr.ConfigurationType;
            if (!configType.QueryProperties().ThatAreInstance().GetAll().Any())
                return configType;
        }

        return null;
    }
}
