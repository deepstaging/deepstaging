// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.CodeFixes.Config;

using Roslyn;

/// <summary>
/// Managed <c>deepstaging.targets</c> file for ItemGroup entries that require late evaluation
/// (e.g., <c>Update</c> with <c>DependentUpon</c> metadata for file nesting).
/// </summary>
public sealed class DeepstagingTargets : ManagedPropsFile
{
    /// <inheritdoc />
    public override string FileName => "deepstaging.targets";

    /// <inheritdoc />
    protected override void ConfigureDefaults(PropsBuilder builder)
    {
        // No structural defaults — ItemGroups are fully managed by code fixes
    }
}
