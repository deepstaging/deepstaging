// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Analyzers;

/// <summary>
/// Reports a diagnostic when [Uses] references a type that is not marked with [EffectsModule].
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("DS0008", "Uses target must be EffectsModule",
    Message = "Type '{0}' referenced in [Uses] is not marked with [EffectsModule]",
    Description =
        "The [Uses] attribute should only reference types that are decorated with [EffectsModule]. Add [EffectsModule] to the target type or remove the [Uses] reference.")]
public sealed class UsesAttributeTargetMustBeEffectsModuleAnalyzer : TypeAnalyzer
{
    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        GetFirstInvalidTarget(type) is not null;

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type) =>
        [GetFirstInvalidTarget(type)!.Value.Name];

    private static ValidSymbol<INamedTypeSymbol>? GetFirstInvalidTarget(ValidSymbol<INamedTypeSymbol> type) =>
        type.UsesAttributes()
            .FirstOrDefault(attr => attr.ModuleType.LacksAttribute<EffectsModuleAttribute>())?
            .ModuleType;
}
