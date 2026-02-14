// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Analyzers;

/// <summary>
/// Reports a diagnostic when [EffectsModule] targets a concrete class instead of an interface or DbContext.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports("DS0004", "EffectsModule target should be interface",
    Message = "Class '{0}' has [EffectsModule] targeting concrete class '{1}' instead of an interface",
    Description =
        "The [EffectsModule] attribute typically targets interfaces for dependency abstraction. Concrete classes are only expected for DbContext types.",
    Severity = DiagnosticSeverity.Warning)]
public sealed class EffectsModuleTargetMustBeInterfaceAnalyzer : TypeAnalyzer
{
    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        GetFirstInvalidTarget(type).HasValue;

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type) =>
        [type.Name, GetFirstInvalidTarget(type).FullyQualifiedName!];

    private static OptionalSymbol<INamedTypeSymbol> GetFirstInvalidTarget(ValidSymbol<INamedTypeSymbol> type) =>
        OptionalSymbol<INamedTypeSymbol>.FromNullable(
            type
                .EffectsModuleAttributes()
                .FirstOrDefault(t =>
                    t.TargetType is { IsInterface: false } &&
                    t.TargetType.IsNotEfDbContext()
                )?.TargetType.Value
        );
}
