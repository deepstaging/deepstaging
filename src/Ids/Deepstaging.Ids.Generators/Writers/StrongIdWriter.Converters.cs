// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using static Deepstaging.Ids.IdConverters;

namespace Deepstaging.Ids.Generators.Writers;

/// <summary>
/// Converter generation for strongly-typed IDs (TypeConverter, JsonConverter, EfCoreValueConverter).
/// </summary>
internal static class ConverterWriter
{
    /// <summary>
    /// Adds all converter nested classes based on IdConverters flags.
    /// </summary>
    internal static TypeBuilder AddConverters(this TypeBuilder builder, StrongIdModel model,
        PropertyBuilder valueProperty)
    {
        return builder
            .If(model.Converters.HasFlag(TypeConverter), b => b.AddTypeConverterClass(model, valueProperty))
            .If(model.Converters.HasFlag(JsonConverter), b => b.AddJsonConverterClass(model, valueProperty))
            .If(model.Converters.HasFlag(EfCoreValueConverter), b => b.AddEfCoreValueConverterClass(model, valueProperty))
            .If(model.Converters.HasFlag(Dapper), b => b.AddDapperTypeHandlerClass(model, valueProperty))
            .If(model.Converters.HasFlag(NewtonsoftJson), b => b.AddNewtonsoftJsonConverterClass(model, valueProperty));
    }
}