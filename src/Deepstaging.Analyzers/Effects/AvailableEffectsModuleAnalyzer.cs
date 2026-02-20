// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Effects;

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Deepstaging.Effects;

/// <summary>
/// Reports an informational diagnostic on <c>[Runtime]</c> classes when there are
/// <c>[EffectsModule]</c>-annotated types in the compilation that are not referenced
/// by any <c>[Uses]</c> attribute on the runtime.
/// </summary>
/// <remarks>
/// Uses <see cref="AnalysisContext.RegisterCompilationStartAction"/> to collect all
/// module-containing types, then reports one diagnostic per unreferenced module type
/// during <see cref="CompilationStartAnalysisContext.RegisterCompilationEndAction"/>.
/// Each diagnostic carries the module type name in <see cref="Diagnostic.Properties"/>
/// so the code fix can construct the <c>[Uses(typeof(X))]</c> attribute.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AvailableEffectsModuleAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Diagnostic ID for available but unreferenced effects modules.
    /// </summary>
    public const string DiagnosticId = "DSRT04";

    /// <summary>
    /// Diagnostic property key for the module type name.
    /// </summary>
    public const string ModuleTypeProperty = "ModuleType";

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Effects module available",
        "Effects module '{0}' is available but not referenced by runtime '{1}'",
        "Usage",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description:
            "A Deepstaging module ([EffectsModule], [EventQueue], or [DispatchModule]) exists in the compilation " +
            "but is not referenced by any [Uses] attribute on this [Runtime] class. Add [Uses(typeof(...))] to wire it in.",
        customTags: [WellKnownDiagnosticTags.CompilationEnd]);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var moduleTypes = new ConcurrentBag<INamedTypeSymbol>();
        var runtimeTypes = new ConcurrentBag<INamedTypeSymbol>();

        context.RegisterSymbolAction(ctx =>
        {
            if (ctx.Symbol is not INamedTypeSymbol type)
                return;

            var valid = ValidSymbol<INamedTypeSymbol>.From(type);

            if (valid.HasAttribute<EffectsModuleAttribute>() ||
                valid.HasAttribute<EventQueueAttribute>() ||
                valid.HasAttribute<DispatchModuleAttribute>())
                moduleTypes.Add(type);

            if (valid.HasAttribute<RuntimeAttribute>())
                runtimeTypes.Add(type);
        }, SymbolKind.NamedType);

        context.RegisterCompilationEndAction(ctx =>
        {
            if (moduleTypes.IsEmpty || runtimeTypes.IsEmpty)
                return;

            foreach (var runtime in runtimeTypes)
            {
                var validRuntime = ValidSymbol<INamedTypeSymbol>.From(runtime);
                var usedModuleTypes = new HashSet<string>(StringComparer.Ordinal);

                foreach (var uses in validRuntime.UsesAttributes())
                {
                    var moduleType = uses.ModuleType;
                    usedModuleTypes.Add(moduleType.Value.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                }

                foreach (var moduleType in moduleTypes)
                {
                    var fqn = moduleType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    if (usedModuleTypes.Contains(fqn))
                        continue;

                    var properties = ImmutableDictionary.CreateRange(new[]
                    {
                        new KeyValuePair<string, string?>(ModuleTypeProperty, moduleType.Name)
                    });

                    ctx.ReportDiagnostic(Diagnostic.Create(
                        Rule,
                        runtime.Locations.FirstOrDefault(),
                        properties,
                        moduleType.Name,
                        runtime.Name));
                }
            }
        });
    }
}
