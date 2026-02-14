// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Ids.Generators.Writers;

internal static class DapperTypeHandlerWriter
{
    /// <summary>
    /// Adds a Dapper SqlMapper.TypeHandler nested class.
    /// </summary>
    internal static TypeBuilder AddDapperTypeHandlerClass(
        this TypeBuilder builder,
        StrongIdModel model,
        PropertyBuilder valueProperty)
    {
        var dapperHandler = TypeBuilder
            .Parse($"public partial class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<{model.TypeName}>")
            .AddMethod(SetValueMethod(model, valueProperty))
            .AddMethod(ParseMethod(model));

        return builder.AddNestedType(dapperHandler);
    }

    private static MethodBuilder SetValueMethod(StrongIdModel model, PropertyBuilder valueProperty) => MethodBuilder
        .Parse($$"""
                 public override void SetValue(
                     global::System.Data.IDbDataParameter parameter,
                     {{model.TypeName}} value)
                 """)
        .WithBody(b => b
            .AddStatements($"parameter.Value = value.{valueProperty.Name};"));

    private static MethodBuilder ParseMethod(StrongIdModel model)
    {
        var typeName = model.TypeName;

        var body = model.BackingType switch
        {
            BackingType.Guid => $$"""
                                  return value switch
                                  {
                                      global::System.Guid guidValue => new {{typeName}}(guidValue),
                                      string stringValue when !string.IsNullOrEmpty(stringValue) && global::System.Guid.TryParse(stringValue, out var result) => new {{typeName}}(result),
                                      _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to {{typeName}}"),
                                  };
                                  """,
            BackingType.Int => $$"""
                                 return value switch
                                 {
                                     int intValue => new {{typeName}}(intValue),
                                     short shortValue => new {{typeName}}(shortValue),
                                     long longValue when longValue is < int.MaxValue and > int.MinValue => new {{typeName}}((int)longValue),
                                     decimal decimalValue when decimalValue is < int.MaxValue and > int.MinValue => new {{typeName}}((int)decimalValue),
                                     string stringValue when !string.IsNullOrEmpty(stringValue) && int.TryParse(stringValue, out var result) => new {{typeName}}(result),
                                     _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to {{typeName}}"),
                                 };
                                 """,
            BackingType.Long => $$"""
                                  return value switch
                                  {
                                      long longValue => new {{typeName}}(longValue),
                                      int intValue => new {{typeName}}(intValue),
                                      short shortValue => new {{typeName}}(shortValue),
                                      decimal decimalValue when decimalValue is < long.MaxValue and > long.MinValue => new {{typeName}}((long)decimalValue),
                                      string stringValue when !string.IsNullOrEmpty(stringValue) && long.TryParse(stringValue, out var result) => new {{typeName}}(result),
                                      _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to {{typeName}}"),
                                  };
                                  """,
            BackingType.String => $$"""
                                    return value switch
                                    {
                                        string stringValue => new {{typeName}}(stringValue),
                                        _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to {{typeName}}"),
                                    };
                                    """,
            _ =>
                $"throw new global::System.InvalidCastException($\"Unable to cast object of type {{value.GetType()}} to {typeName}\");"
        };

        return MethodBuilder
            .Parse($"public override {typeName} Parse(object value)")
            .WithBody(b => b.AddStatements(body));
    }
}
