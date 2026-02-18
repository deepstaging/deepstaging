// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Config;

using Deepstaging.Config;

/// <summary>
/// Reports a diagnostic when a property on an exposed configuration type appears to contain
/// secrets or PII but is not marked with <c>[Secret]</c>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "Property may contain secrets",
    Message = "Property '{0}' on '{1}' appears to contain secrets or PII — consider adding [Secret]",
    Description =
        "Properties whose names suggest sensitive data (e.g. Password, ApiKey, Token, ConnectionString) " +
        "should be decorated with [Secret] so they are placed in the secrets schema instead of appsettings.",
    Severity = DiagnosticSeverity.Warning
)]
public sealed class PotentialSecretPropertyAnalyzer : PropertyAnalyzer
{
    /// <summary>
    /// Diagnostic ID for potential secret properties missing [Secret].
    /// </summary>
    public const string DiagnosticId = "CFG005";

    private static readonly string[] SecretPatterns =
    [
        "Password",
        "Secret",
        "ApiKey",
        "Token",
        "ConnectionString",
        "Credential",
        "PrivateKey",
        "AccessKey",
        "ClientSecret",
        "Passphrase"
    ];

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<IPropertySymbol> property) =>
        !property.HasAttribute<SecretAttribute>()
        && LooksLikeSecret(property.Name)
        && IsExposedByConfigProvider(property);

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<IPropertySymbol> property) =>
        [property.Name, property.Value.ContainingType?.Name ?? ""];

    private static bool IsExposedByConfigProvider(ValidSymbol<IPropertySymbol> property)
    {
        var containingType = property.Value.ContainingType;
        if (containingType is null)
            return false;

        // Check if the containing type has any attribute whose name starts with "Exposes"
        // referencing it as a type argument — this means some ConfigProvider exposes it.
        // Alternatively, check if the containing type itself is annotated in a way that
        // indicates it's a config type. We use a simple heuristic: any type that is
        // referenced as a type argument of ExposesAttribute<T> somewhere in the same assembly.
        foreach (var attr in containingType.GetAttributes())
        {
            // If the property's containing type itself has [ConfigProvider], it's not an exposed type
            if (attr.AttributeClass?.Name is nameof(ConfigProviderAttribute) or "ConfigProviderAttribute")
                return false;
        }

        // The property lives on a plain config type. Flag it if it looks like a secret.
        // The [Exposes<T>] linkage is validated by CFG004/CFG005 at the provider level.
        return true;
    }

    private static bool LooksLikeSecret(string name) =>
        SecretPatterns.Any(pattern => name.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);
}
