// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.LocalRefs;

/// <summary>
/// Type references for <c>Microsoft.Extensions.Configuration.UserSecrets</c>.
/// </summary>
internal static class UserSecretsRefs
{
    public static NamespaceRef Namespace => NamespaceRef.From("Microsoft.Extensions.Configuration");

    // UserSecretsConfigurationExtensions.AddUserSecrets(builder, assembly, optional)
    public static string AddUserSecrets(ExpressionRef builder, ExpressionRef assembly) =>
        $"{builder}.AddUserSecrets({assembly}, optional: true)";
}
