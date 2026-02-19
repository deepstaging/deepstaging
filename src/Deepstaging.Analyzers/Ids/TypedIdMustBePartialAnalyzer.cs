// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Ids;

/// <summary>
/// Reports a diagnostic when a struct with [TypedId] is not declared as partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "TypedId struct must be partial",
    Message = "Struct '{0}' has [TypedId] attribute but is not declared as partial",
    Description =
        "Structs decorated with [TypedId] must be declared as partial because the source generator emits additional partial struct members."
)]
public sealed class TypedIdMustBePartialAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing partial modifier on TypedId struct.
    /// </summary>
    public const string DiagnosticId = "DSID01";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<TypedIdAttribute>() && type is { IsPartial: false };
}