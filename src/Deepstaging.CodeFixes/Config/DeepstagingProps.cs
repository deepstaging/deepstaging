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

    /// <inheritdoc />
    protected override void ConfigureDefaults(PropsBuilder builder) => builder
        .Property("CompilerGeneratedFilesOutputPath", "!generated")
        .ItemGroup(items => items
            .Remove("Compile", "!generated/**")
            .Include("None", "!generated/**"));
}