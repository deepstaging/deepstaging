// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using static Deepstaging.Ids.IdConverters;

namespace Deepstaging.Generators.Writers.Ids;

using Projection.Ids.Models;

/// <summary>
/// Converter generation for typed IDs. TypeConverter is always emitted;
/// JsonConverter and EfCoreValueConverter are opt-in via <see cref="IdConverters"/> flags.
/// </summary>
internal static class ConverterWriter
{
    /// <summary>
    /// Adds converter nested classes. TypeConverter is always generated;
    /// others are controlled by <see cref="IdConverters"/> flags.
    /// </summary>
    internal static TypeBuilder AddConverters(this TypeBuilder builder, TypedIdModel model,
        PropertyBuilder valueProperty)
    {
        return builder
            .AddTypeConverterClass(model, valueProperty)
            .If(model.Converters.HasFlag(JsonConverter), b => b.AddJsonConverterClass(model, valueProperty))
            .If(model.Converters.HasFlag(EfCoreValueConverter), b => b.AddEfCoreValueConverterClass(model, valueProperty));
    }
}
