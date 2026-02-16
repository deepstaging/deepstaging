// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Ids;

using Projection.Ids.Models;

internal static class NewtonsoftJsonConverterWriter
{
    /// <summary>
    /// Adds a Newtonsoft.Json JsonConverter nested class.
    /// </summary>
    internal static TypeBuilder AddNewtonsoftJsonConverterClass(
        this TypeBuilder builder,
        StrongIdModel model,
        PropertyBuilder valueProperty)
    {
        var typeName = model.TypeName;

        var newtonsoftConverter = TypeBuilder
            .Parse($"public partial class {typeName}NewtonsoftJsonConverter : global::Newtonsoft.Json.JsonConverter")
            .AddMethod(CanConvertMethod(typeName))
            .AddMethod(WriteJsonMethod(typeName, valueProperty))
            .AddMethod(ReadJsonMethod(model, typeName));

        return builder
            .WithAttribute("global::Newtonsoft.Json.JsonConverter", a => a
                .WithArgument($"typeof({model.TypeName}NewtonsoftJsonConverter)"))
            .AddNestedType(newtonsoftConverter);
    }

    private static MethodBuilder CanConvertMethod(string typeName)
    {
        return MethodBuilder
            .Parse("public override bool CanConvert(global::System.Type objectType)")
            .WithBody(b => b.AddStatements($"return objectType == typeof({typeName});"));
    }

    private static MethodBuilder ReadJsonMethod(StrongIdModel model, string typeName)
    {
        return MethodBuilder
            .Parse(
                """
                public override object? ReadJson(
                    global::Newtonsoft.Json.JsonReader reader, 
                    global::System.Type objectType, 
                    object? existingValue, 
                    global::Newtonsoft.Json.JsonSerializer serializer)
                """)
            .WithBody(b =>
            {
                var body = ReadJsonBody(model, typeName);
                return b.AddStatements(body);
            });
    }

    private static MethodBuilder WriteJsonMethod(string typeName, PropertyBuilder valueProperty)
    {
        return MethodBuilder
            .Parse(
                """
                public override void WriteJson(
                    global::Newtonsoft.Json.JsonWriter writer,
                    object? value, 
                    global::Newtonsoft.Json.JsonSerializer serializer)
                """)
            .WithBody(b => b
                .AddStatements($"serializer.Serialize(writer, value is {typeName} id ? id.{valueProperty.Name} : null);"));
    }

    private static string ReadJsonBody(StrongIdModel model, string typeName)
    {
        return model.BackingType switch
        {
            BackingType.Guid => $$"""
                                  var guid = serializer.Deserialize<global::System.Guid?>(reader);
                                  return guid.HasValue ? new {{typeName}}(guid.Value) : null;
                                  """,
            BackingType.Int => $$"""
                                 var result = serializer.Deserialize<int?>(reader);
                                 return result.HasValue ? new {{typeName}}(result.Value) : null;
                                 """,
            BackingType.Long => $$"""
                                  var result = serializer.Deserialize<long?>(reader);
                                  return result.HasValue ? new {{typeName}}(result.Value) : null;
                                  """,
            BackingType.String => $$"""
                                    if (objectType == typeof({{typeName}}?))
                                    {
                                        var value = serializer.Deserialize<string?>(reader);
                                        return value is null ? null : new {{typeName}}(value);
                                    }

                                    return new {{typeName}}(serializer.Deserialize<string>(reader)!);
                                    """,
            _ => "return null;"
        };
    }
}
