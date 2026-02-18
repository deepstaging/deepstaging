// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Effects;

using Deepstaging.Effects;

/// <summary>
/// Reports a diagnostic when [EffectsModule] targets a type that has no methods to lift into effects.
/// Suggests using [Capability] instead for non-effectful dependencies like configuration providers.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "EffectsModule target has no methods",
    Message = "[EffectsModule] targeting '{0}' will produce no effect methods because the type has no methods. Use [Capability(typeof({0}))] instead to expose it as a runtime dependency without effect wrapping.",
    Description =
        "The [EffectsModule] attribute is intended for types whose methods are lifted into the Eff effect system. " +
        "Types with only properties (such as generated configuration providers) should use [Capability] instead, " +
        "which generates the IHas* capability interface and wires the dependency into the runtime without generating effect methods.",
    Severity = DiagnosticSeverity.Warning
)]
public sealed class EffectsModuleTargetHasNoMethodsAnalyzer : TypeAnalyzer
{
    /// <summary>The diagnostic identifier for this analyzer.</summary>
    public const string DiagnosticId = "DS0010";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        GetFirstEmptyTarget(type) is not null;

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type) =>
        [GetFirstEmptyTarget(type)!.Value.Name];

    private static ValidSymbol<INamedTypeSymbol>? GetFirstEmptyTarget(ValidSymbol<INamedTypeSymbol> type) =>
        type.EffectsModuleAttributes()
            .Where(attr => attr.HasValidTargetType && attr.TargetType.IsNotEfDbContext())
            .FirstOrDefault(attr => !attr.TargetType.QueryMethods().Any())?
            .TargetType;
}
