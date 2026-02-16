// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Config;

using Deepstaging.Config;
using Deepstaging.Projection.Config;

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
public sealed class PotentialSecretPropertyAnalyzer : TypeAnalyzer
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
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<ConfigProviderAttribute>() && GetFirstSuspectProperty(type) is not null;

    /// <inheritdoc />
    protected override object[] GetMessageArgs(ValidSymbol<INamedTypeSymbol> type)
    {
        var (property, configType) = GetFirstSuspectProperty(type)!.Value;
        return [property.Name, configType.Name];
    }

    private static (ValidSymbol<IPropertySymbol> Property, ValidSymbol<INamedTypeSymbol> ConfigType)? GetFirstSuspectProperty(
        ValidSymbol<INamedTypeSymbol> type)
    {
        foreach (var attr in type.ExposesAttributes())
        {
            var configType = attr.ConfigurationType;

            foreach (var property in configType.QueryProperties().ThatAreInstance().GetAll())
            {
                if (property.HasAttribute<SecretAttribute>())
                    continue;

                if (LooksLikeSecret(property.Name))
                    return (property, configType);
            }
        }

        return null;
    }

    private static bool LooksLikeSecret(string name) =>
        SecretPatterns.Any(pattern => name.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);
}
