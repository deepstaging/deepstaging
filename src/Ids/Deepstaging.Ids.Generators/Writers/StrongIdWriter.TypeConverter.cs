// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Converters;

namespace Deepstaging.Ids.Generators.Writers;

internal static class TypeConverterWriter
{
    /// <summary>
    /// Adds a System.ComponentModel.TypeConverter nested class.
    /// </summary>
    internal static TypeBuilder AddTypeConverterClass(
        this TypeBuilder builder,
        StrongIdModel model,
        PropertyBuilder valueProperty)
    {
        return builder.WithTypeConverter(
            $"{model.TypeName}TypeConverter",
            CanConvertFromBody(model),
            ConvertFromBody(model),
            CanConvertToBody(model),
            ConvertToBody(model, valueProperty));
    }

    private static string CanConvertFromBody(StrongIdModel model) =>
        model.BackingType switch
        {
            BackingType.Guid =>
                """
                return sourceType == typeof(global::System.Guid)
                    || sourceType == typeof(string)
                    || base.CanConvertFrom(context, sourceType);
                """,
            BackingType.Int =>
                """
                return sourceType == typeof(int)
                    || sourceType == typeof(string)
                    || base.CanConvertFrom(context, sourceType);
                """,
            BackingType.Long =>
                """
                return sourceType == typeof(long)
                    || sourceType == typeof(string)
                    || base.CanConvertFrom(context, sourceType);
                """,
            BackingType.String =>
                """
                return sourceType == typeof(string)
                    || base.CanConvertFrom(context, sourceType);
                """,
            _ => "return base.CanConvertFrom(context, sourceType);"
        };

    private static string ConvertFromBody(StrongIdModel model) =>
        model.BackingType switch
        {
            BackingType.Guid => $$"""
                return value switch
                {
                    global::System.Guid guidValue => new {{model.TypeName}}(guidValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue)
                        && global::System.Guid.TryParse(stringValue, out var result) => new {{model.TypeName}}(result),
                    _ => base.ConvertFrom(context, culture, value),
                };
                """,
            BackingType.Int => $$"""
                return value switch
                {
                    int intValue => new {{model.TypeName}}(intValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue)
                        && int.TryParse(stringValue, out var result) => new {{model.TypeName}}(result),
                    _ => base.ConvertFrom(context, culture, value),
                };
                """,
            BackingType.Long => $$"""
                return value switch
                {
                    long longValue => new {{model.TypeName}}(longValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue)
                        && long.TryParse(stringValue, out var result) => new {{model.TypeName}}(result),
                    _ => base.ConvertFrom(context, culture, value),
                };
                """,
            BackingType.String => $$"""
                return value switch
                {
                    string stringValue => new {{model.TypeName}}(stringValue),
                    _ => base.ConvertFrom(context, culture, value),
                };
                """,
            _ => "return base.ConvertFrom(context, culture, value);"
        };

    private static string CanConvertToBody(StrongIdModel model) =>
        model.BackingType switch
        {
            BackingType.Guid =>
                """
                return sourceType == typeof(global::System.Guid)
                    || sourceType == typeof(string)
                    || base.CanConvertTo(context, sourceType);
                """,
            BackingType.Int =>
                """
                return sourceType == typeof(int)
                    || sourceType == typeof(string)
                    || base.CanConvertTo(context, sourceType);
                """,
            BackingType.Long =>
                """
                return sourceType == typeof(long)
                    || sourceType == typeof(string)
                    || base.CanConvertTo(context, sourceType);
                """,
            BackingType.String =>
                """
                return sourceType == typeof(string)
                    || base.CanConvertTo(context, sourceType);
                """,
            _ => "return base.CanConvertTo(context, sourceType);"
        };

    private static string ConvertToBody(StrongIdModel model, PropertyBuilder valueProperty) =>
        model.BackingType switch
        {
            BackingType.Guid => $$"""
                if (value is {{model.TypeName}} idValue)
                {
                    if (destinationType == typeof(global::System.Guid))
                        return idValue.{{valueProperty.Name}};

                    if (destinationType == typeof(string))
                        return idValue.{{valueProperty.Name}}.ToString();
                }

                return base.ConvertTo(context, culture, value, destinationType);
                """,
            BackingType.Int => $$"""
                if (value is {{model.TypeName}} idValue)
                {
                    if (destinationType == typeof(int))
                        return idValue.{{valueProperty.Name}};

                    if (destinationType == typeof(string))
                        return idValue.{{valueProperty.Name}}.ToString();
                }

                return base.ConvertTo(context, culture, value, destinationType);
                """,
            BackingType.Long => $$"""
                if (value is {{model.TypeName}} idValue)
                {
                    if (destinationType == typeof(long))
                        return idValue.{{valueProperty.Name}};

                    if (destinationType == typeof(string))
                        return idValue.{{valueProperty.Name}}.ToString();
                }

                return base.ConvertTo(context, culture, value, destinationType);
                """,
            BackingType.String => $$"""
                if (value is {{model.TypeName}} idValue)
                {
                    if (destinationType == typeof(string))
                        return idValue.{{valueProperty.Name}};
                }

                return base.ConvertTo(context, culture, value, destinationType);
                """,
            _ => "return base.ConvertTo(context, culture, value, destinationType);"
        };
}
