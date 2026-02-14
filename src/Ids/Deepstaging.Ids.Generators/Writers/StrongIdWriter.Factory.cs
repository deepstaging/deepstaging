// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Patterns;

namespace Deepstaging.Ids.Generators.Writers;

/// <summary>
/// Factory and parse method generation for strongly-typed IDs.
/// </summary>
internal static class FactoryWriter
{
    /// <summary>
    /// Adds factory methods based on backing type.
    /// </summary>
    internal static TypeBuilder AddFactoryMethods(this TypeBuilder builder, StrongIdModel model)
    {
        var typeName = model.TypeName;

        return model.BackingType switch
        {
            BackingType.Guid => builder
                .WithEmptyAndNewFactory(
                    $"new {typeName}(global::System.Guid.Empty)",
                    $"new {typeName}(global::System.Guid.NewGuid())"),
            BackingType.Int => builder
                .WithEmptyFactory($"new {typeName}(0)"),
            BackingType.Long => builder
                .WithEmptyFactory($"new {typeName}(0)"),
            BackingType.String => builder
                .WithEmptyFactory($"new {typeName}(string.Empty)"),
            _ => builder
        };
    }

    /// <summary>
    /// Adds Parse(string) method.
    /// </summary>
    internal static TypeBuilder AddParseMethods(this TypeBuilder builder, StrongIdModel model)
    {
        var typeName = model.TypeName;

        return model.BackingType switch
        {
            BackingType.Guid => builder
                .AddMethod(MethodBuilder
                    .Parse($"public static {typeName} Parse(string input)")
                    .WithExpressionBody($"new(global::System.Guid.Parse(input))")),

            BackingType.Int => builder
                .AddMethod(MethodBuilder
                    .Parse($"public static {typeName} Parse(string input)")
                    .WithExpressionBody($"new(int.Parse(input))")),

            BackingType.Long => builder
                .AddMethod(MethodBuilder
                    .Parse($"public static {typeName} Parse(string input)")
                    .WithExpressionBody($"new(long.Parse(input))")),

            BackingType.String => builder
                .AddMethod(MethodBuilder
                    .Parse($"public static {typeName} Parse(string input)")
                    .WithExpressionBody($"new(input)")),

            _ => builder
        };
    }
}
