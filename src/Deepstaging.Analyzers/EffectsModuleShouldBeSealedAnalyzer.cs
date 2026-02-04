using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Analyzers;

/// <summary>
/// Reports a diagnostic when a class with [EffectsModule] is not declared as sealed.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "EffectsModule class should be sealed",
    Message = "Class '{0}' has [EffectsModule] attribute but is not declared as sealed",
    Description = "Classes decorated with [EffectsModule] are typically leaf types and should be sealed to prevent inheritance.",
    Severity = DiagnosticSeverity.Warning)]
public sealed class EffectsModuleShouldBeSealedAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing sealed modifier on EffectsModule class.
    /// </summary>
    public const string DiagnosticId = "DS0009";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<EffectsModuleAttribute>() && type is { IsSealed: false, IsStatic: false };
}
