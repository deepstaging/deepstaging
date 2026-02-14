// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Analyzers;

/// <summary>
/// Reports a diagnostic when a class has multiple [EffectsModule] attributes targeting the same type.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("DS0005", "Duplicate EffectsModule target type",
    Message = "Class '{0}' has multiple [EffectsModule] attributes targeting '{1}'",
    Description = "Each target type should only be specified once in [EffectsModule] attributes on the same class.")]
public sealed class EffectsModuleDuplicateTargetAnalyzer : TypeAnalyzer
{
    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        var targetTypes = GetTargetTypes(type);
        return targetTypes.Count != targetTypes.Distinct(SymbolEqualityComparer.Default).Count();
    }

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type)
    {
        var duplicateTarget = GetTargetTypes(type)
            .GroupBy(t => t, SymbolEqualityComparer.Default)
            .First(g => g.Count() > 1)
            .Key!;

        return [type.Name, duplicateTarget.ToDisplayString()];
    }

    private static List<INamedTypeSymbol> GetTargetTypes(ValidSymbol<INamedTypeSymbol> type) =>
    [
        ..type.EffectsModuleAttributes()
            .Select(x => x.TargetType.Value)
    ];
}