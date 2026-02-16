// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Ids;

/// <summary>
/// Specifies which type converters to generate for a strongly-typed ID.
/// </summary>
[Flags]
public enum IdConverters
{
    /// <summary>
    /// No converters are generated.
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

    /// <summary>
    /// Generates a <see cref="System.ComponentModel.TypeConverter"/> for design-time support.
    /// </summary>
    TypeConverter = 1 << 2,

    /// <summary>
    /// Generates a Dapper SqlMapper.TypeHandler for database integration.
    /// </summary>
    Dapper = 1 << 3,

    /// <summary>
    /// Generates a Newtonsoft.Json JsonConverter.
    /// </summary>
    NewtonsoftJson = 1 << 4,

    /// <summary>
    /// Generates all available converters.
    /// </summary>
    All = JsonConverter | EfCoreValueConverter | TypeConverter | Dapper | NewtonsoftJson
}