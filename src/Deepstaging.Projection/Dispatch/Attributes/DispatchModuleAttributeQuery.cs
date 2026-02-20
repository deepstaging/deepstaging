// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Dispatch.Attributes;

using Attr = DispatchModuleAttribute;

/// <summary>
/// A queryable wrapper over <see cref="DispatchModuleAttribute"/> data.
/// </summary>
/// <param name="AttributeData">The underlying Roslyn attribute data.</param>
public sealed record DispatchModuleAttributeQuery(AttributeData AttributeData) : AttributeQuery(AttributeData)
{
    /// <summary>
    /// Gets whether auto-commit is enabled for command dispatch. Defaults to <c>true</c>.
    /// </summary>
    public bool AutoCommit => NamedArg<bool>(nameof(Attr.AutoCommit))
        .OrDefault(true);
}
