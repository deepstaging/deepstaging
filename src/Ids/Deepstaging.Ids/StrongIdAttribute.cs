// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Ids;

/// <summary>
/// Marks a partial struct as a strongly-typed ID, triggering code generation
/// for equality, conversion operators, and optional serialization converters.
/// </summary>
/// <example>
/// <code>
/// [StrongId]
/// public partial struct UserId;
/// 
/// [StrongId(BackingType = BackingType.Int)]
/// public partial struct OrderId;
/// 
/// [StrongId(Converters = IdConverters.JsonConverter | IdConverters.EfCoreValueConverter)]
/// public partial struct CustomerId;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class StrongIdAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the primitive type used to store the ID value.
    /// Default is <see cref="BackingType.Guid"/>.
    /// </summary>
    public BackingType BackingType { get; set; } = BackingType.Guid;

    /// <summary>
    /// Gets or sets which type converters to generate.
    /// Default is <see cref="IdConverters.None"/>.
    /// </summary>
    public IdConverters Converters { get; set; } = IdConverters.None;
}
