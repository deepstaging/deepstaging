using Deepstaging.Projection;
using Deepstaging.Projection.Attributes;
using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Analyzers;

/// <summary>
/// Reports a diagnostic when [Uses] references a type that is not marked with [EffectsModule].
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "Uses target must be EffectsModule",
    Message = "Type '{0}' referenced in [Uses] is not marked with [EffectsModule]",
    Description =
        "The [Uses] attribute should only reference types that are decorated with [EffectsModule]. Add [EffectsModule] to the target type or remove the [Uses] reference.")]
public sealed class UsesAttributeTargetMustBeEffectsModuleAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for Uses target not being an EffectsModule.
    /// </summary>
    public const string DiagnosticId = "DS0008";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        GetFirstInvalidTarget(type) is not null;

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type) =>
        [GetFirstInvalidTarget(type)!.Value.Name];

    private static ValidSymbol<INamedTypeSymbol>? GetFirstInvalidTarget(ValidSymbol<INamedTypeSymbol> type)
    {
        return type.UsesAttributes()
            .FirstOrDefault(attr => attr.ModuleType.LacksAttribute<EffectsModuleAttribute>())?
            .ModuleType;
    }
}