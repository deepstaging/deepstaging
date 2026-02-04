using Deepstaging.Projection;
using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Analyzers;

/// <summary>
/// Reports a diagnostic when [EffectsModule] Exclude references a method that doesn't exist on the target type.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "Excluded method not found",
    Message = "Method '{0}' specified in Exclude does not exist on target type '{1}'",
    Description =
        "The Exclude property references a method name that cannot be found on the target type. Check for typos or remove the invalid entry.")]
public sealed class EffectsModuleExcludeMethodNotFoundAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for excluded method not found.
    /// </summary>
    public const string DiagnosticId = "DS0006";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        return GetFirstInvalidExclude(type) is not null;
    }

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type)
    {
        var (methodName, targetType) = GetFirstInvalidExclude(type)!.Value;
        return [methodName, targetType.ToDisplayString()];
    }

    private static (string MethodName, ITypeSymbol TargetType)? GetFirstInvalidExclude(
        ValidSymbol<INamedTypeSymbol> type)
    {
        var attribute = type
            .EffectsModuleAttributes()
            .FirstOrDefault(attr => attr.Exclude.Any());

        if (attribute is null)
            return null;

        var targetMethods = new HashSet<string>(attribute.TargetType.QueryMethods().Select(method => method.Name));
        var methodName = attribute.Exclude.FirstOrDefault(methodName => !targetMethods.Contains(methodName));
        return methodName is not null ? (methodName, attribute.TargetType.Value) : null;
    }
}