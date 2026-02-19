// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes.Config;

using Roslyn;

/// <summary>
/// Managed <c>deepstaging.props</c> file with standard defaults for all Deepstaging generators.
/// </summary>
public sealed class DeepstagingProps : ManagedPropsFile
{
    /// <inheritdoc />
    public override string FileName => "deepstaging.props";
    
    /// <summary>The label for the settings property group in the props file.</summary>
    public const string SettingsPropertyGroup = "Settings";

    /// <summary>The MSBuild property name for the user secrets identifier.</summary>
    public const string UserSecretsIdProperty = "UserSecretsId";

    /// <inheritdoc />
    protected override void ConfigureDefaults(PropsBuilder builder)
    {
        // No structural defaults — properties like CompilerGeneratedFilesOutputPath
        // and !generated/** items are handled by the NuGet package's build/Deepstaging.props
        // and build/Deepstaging.targets. The local props file only holds per-project settings.
    }
}