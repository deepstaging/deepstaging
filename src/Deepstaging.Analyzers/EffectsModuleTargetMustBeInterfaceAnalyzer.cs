using Deepstaging.Projection;
using Deepstaging.Projection.Attributes;
using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Analyzers;

/// <summary>
/// Reports a diagnostic when [EffectsModule] targets a concrete class instead of an interface or DbContext.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "EffectsModule target should be interface",
    Message = "Class '{0}' has [EffectsModule] targeting concrete class '{1}' instead of an interface",
    Description =
        "The [EffectsModule] attribute typically targets interfaces for dependency abstraction. Concrete classes are only expected for DbContext types.",
    Severity = DiagnosticSeverity.Warning)]
public sealed class EffectsModuleTargetMustBeInterfaceAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for EffectsModule targeting non-interface.
    /// </summary>
    public const string DiagnosticId = "DS0004";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        return GetFirstInvalidTarget(type).HasValue;
    }

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type)
    {
        var invalidTarget = GetFirstInvalidTarget(type);
        return [type.Name, invalidTarget.FullyQualifiedName!];
    }

    private static OptionalSymbol<INamedTypeSymbol> GetFirstInvalidTarget(ValidSymbol<INamedTypeSymbol> type)
    {
        return OptionalSymbol<INamedTypeSymbol>.FromNullable(
            type
                .EffectsModuleAttributes()
                .FirstOrDefault(t =>
                    t.TargetType is { IsInterface: false } &&
                    t.TargetType.IsNotEfDbContext()
                )?.TargetType.Value
        );
    }
}