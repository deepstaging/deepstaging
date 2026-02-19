// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Analyzers.Config;

using Deepstaging.Config;
using Deepstaging.Projection.Config;

/// <summary>
/// Reports a diagnostic when a <c>[ConfigProvider]</c> class exposes <c>[Secret]</c> properties
/// but the project does not have a <c>UserSecretsId</c> configured.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(
    DiagnosticId,
    "UserSecretsId may be missing",
    Message = "Class '{0}' exposes [Secret] properties but the assembly has no UserSecretsIdAttribute — add a UserSecretsId via the code fix",
    Description =
        "When a ConfigProvider exposes properties marked with [Secret], the generated code calls " +
        "AddUserSecrets. This requires a <UserSecretsId> element in deepstaging.props or the project file.",
    Severity = DiagnosticSeverity.Error
)]
public sealed class MissingUserSecretsIdAnalyzer : TypeAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing UserSecretsId when [Secret] properties exist.
    /// </summary>
    public const string DiagnosticId = "DSCFG07";

    /// <inheritdoc />
    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type)
    {
        if (!type.HasAttribute<ConfigProviderAttribute>())
            return false;

        var hasSecrets = type.ExposesAttributes()
            .Any(attr => attr.ConfigurationType
                .QueryProperties().ThatAreInstance().GetAll()
                .Any(p => p.HasAttribute<SecretAttribute>()));

        if (!hasSecrets)
            return false;

        return !type.Value.ContainingAssembly.HasAttribute("UserSecretsIdAttribute");
    }
}
