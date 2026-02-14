// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Effects.Projection.Models;

namespace Deepstaging.Effects.Testing.Analyzers;

/// <summary>
/// Reports an info diagnostic for each capability on a <c>[TestRuntime&lt;T&gt;]</c> class.
/// Each diagnostic offers to generate success/error scenario helper methods.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "Generate capability test scenarios",
    Message = "Capability '{0}' on '{1}' — generate success/error scenarios",
    Description =
        "Classes decorated with [TestRuntime<T>] expose capabilities from the production runtime. " +
        "Use the offered code fix to scaffold WithXxxSuccess() and WithXxxError() helper methods.",
    Severity = DiagnosticSeverity.Info)]
public sealed class TestRuntimeCapabilityInfoAnalyzer : MultiDiagnosticTypeAnalyzer<RuntimeCapabilityModel>
{
    /// <summary>
    /// Diagnostic ID for the test runtime capability info diagnostic.
    /// </summary>
    public const string DiagnosticId = "DS0010";

    /// <inheritdoc />
    protected override IEnumerable<RuntimeCapabilityModel> GetDiagnosticItems(ValidSymbol<INamedTypeSymbol> symbol)
    {
        if (!symbol.HasAttribute(typeof(TestRuntimeAttribute<>)))
            return [];

        return symbol.QueryTestRuntimeModel().Capabilities;
    }

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> symbol, RuntimeCapabilityModel item) =>
        [item.PropertyName, symbol.Name];

    /// <inheritdoc />
    protected override ImmutableDictionary<string, string?> GetProperties(ValidSymbol<INamedTypeSymbol> symbol, RuntimeCapabilityModel item) =>
        ImmutableDictionary<string, string?>.Empty.Add("CapabilityName", item.PropertyName);
}
