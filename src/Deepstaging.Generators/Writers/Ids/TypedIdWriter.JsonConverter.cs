// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Ids;

using Projection.Ids.Models;
using Roslyn.Emit.Converters;
using Roslyn.Expressions;

internal static class JsonConverterWriter
{
    /// <summary>
    /// Adds a System.Text.Json.JsonConverter nested class.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="model">The TypedId model.</param>
    /// <param name="valueProperty"></param>
    internal static TypeBuilder AddJsonConverterClass(this TypeBuilder builder,
        TypedIdModel model,
        PropertyBuilder valueProperty)
    {
        var typeName = builder.Name;

        return builder.WithJsonConverter(
            $"{model.TypeName}SystemTextJsonConverter",
            GetReadExpression(model.BackingType),
            GetWriteExpression(model.BackingType, valueProperty),
            GetReadAsPropertyNameExpression(typeName, model.BackingType),
            GetWriteAsPropertyNameExpression(model.BackingType, valueProperty),
            true
        );
    }

    private static string GetReadExpression(BackingType backingType) =>
        backingType switch
        {
            BackingType.Guid => "new(reader.GetGuid())",
            BackingType.Int => "new(reader.GetInt32())",
            BackingType.Long => "new(reader.GetInt64())",
            BackingType.String => "new(reader.GetString()!)",
            _ => throw new ArgumentOutOfRangeException(nameof(backingType), backingType, null)
        };

    private static string GetWriteExpression(BackingType backingType, PropertyBuilder valueProperty) =>
        backingType switch
        {
            BackingType.Guid => $"writer.WriteStringValue(value.{valueProperty.Name})",
            BackingType.Int or BackingType.Long => $"writer.WriteNumberValue(value.{valueProperty.Name})",
            BackingType.String => $"writer.WriteStringValue(value.{valueProperty.Name})",
            _ => throw new ArgumentOutOfRangeException(nameof(backingType), backingType, null)
        };

    static void Test()
    {
    }


    private static string GetReadAsPropertyNameExpression(string typeName, BackingType backingType)
    {
        var throwExpression = ExceptionExpression.ThrowNew(ExceptionTypes.Format, $"\"The string for the {typeName} property was null\"");

        return backingType switch
        {
            BackingType.Guid => $"new(global::System.Guid.Parse(reader.GetString() ?? {throwExpression})",
            BackingType.Int => $"new(int.Parse(reader.GetString() ?? {throwExpression})",
            BackingType.Long => $"new(long.Parse(reader.GetString() ?? {throwExpression})",
            BackingType.String => "new(reader.GetString()!)", _ => throw new ArgumentOutOfRangeException(nameof(backingType), backingType, null)
        };
    }

    private static string GetWriteAsPropertyNameExpression(BackingType backingType, PropertyBuilder valueProperty) =>
        backingType switch
        {
            BackingType.String => $"writer.WritePropertyName(value.{valueProperty.Name} ?? string.Empty)",
            _ => $"writer.WritePropertyName(value.{valueProperty.Name}.ToString())"
        };
}