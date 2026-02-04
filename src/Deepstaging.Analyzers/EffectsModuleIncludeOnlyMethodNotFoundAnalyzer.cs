using Deepstaging.Projection;
using Deepstaging.Roslyn;
using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Analyzers;

/// <summary>
/// Reports a diagnostic when [EffectsModule] IncludeOnly references a method that doesn't exist on the target type.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "IncludeOnly method not found",
    Message = "Method '{0}' specified in IncludeOnly does not exist on target type '{1}'",
    Description =
        "The IncludeOnly property references a method name that cannot be found on the target type. Check for typos or remove the invalid entry.")]
public sealed class EffectsModuleIncludeOnlyMethodNotFoundAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for IncludeOnly method not found.
    /// </summary>
    public const string DiagnosticId = "DS0007";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        GetFirstInvalidIncludeOnly(type) is not null;

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type)
    {
        var (methodName, targetType) = GetFirstInvalidIncludeOnly(type)!.Value;
        return [methodName, targetType.ToDisplayString()];
    }

    private static (string MethodName, ITypeSymbol TargetType)? GetFirstInvalidIncludeOnly(
        ValidSymbol<INamedTypeSymbol> type)
    {
        var attribute = type
            .EffectsModuleAttributes()
            .FirstOrDefault(attr => attr.IncludeOnly.Any());

        if (attribute is null)
            return null;

        var targetMethods = new HashSet<string>(attribute.TargetType.QueryMethods().Select(method => method.Name));
        var methodName = attribute.IncludeOnly.FirstOrDefault(name => !targetMethods.Contains(name));
        return methodName is not null ? (methodName, attribute.TargetType.Value) : null;
    }
}
