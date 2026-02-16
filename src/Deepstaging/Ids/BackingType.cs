// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Ids;

/// <summary>
/// Specifies the backing primitive type for a strongly-typed ID.
/// </summary>
public enum BackingType
{
    /// <summary>
    /// Uses <see cref="System.Guid"/> as the backing type (default).
    /// </summary>
    Guid = 0,

    /// <summary>
    /// Uses <see cref="int"/> as the backing type.
    /// </summary>
    Int = 1,

    /// <summary>
    /// Uses <see cref="long"/> as the backing type.
    /// </summary>
    Long = 2,

    /// <summary>
    /// Uses <see cref="string"/> as the backing type.
    /// </summary>
    String = 3
}