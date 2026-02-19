// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Ids;

/// <summary>
/// Specifies which type converters to generate for a typed ID.
/// A <see cref="System.ComponentModel.TypeConverter"/> is always generated regardless of this setting.
/// </summary>
[Flags]
public enum IdConverters
{
    /// <summary>
    /// No additional converters are generated.
    /// </summary>
    None = 0,

    /// <summary>
    /// Generates a System.Text.Json JsonConverter.
    /// </summary>
    JsonConverter = 1 << 0,

    /// <summary>
    /// Generates an Entity Framework Core ValueConverter.
    /// </summary>
    EfCoreValueConverter = 1 << 1,
}