// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Converters;

namespace Deepstaging.Ids.Generators.Writers;

internal static class EfCoreValueConverterWriter
{
    /// <summary>
    /// Adds an Entity Framework Core ValueConverter nested class.
    /// </summary>
    internal static TypeBuilder AddEfCoreValueConverterClass(
        this TypeBuilder builder,
        StrongIdModel model,
        PropertyBuilder valueProperty)
    {
        return builder.WithEfCoreValueConverter(
            model.BackingTypeSymbol.FullyQualifiedName,
            $"id => id.{valueProperty.Name}",
            $"value => new {builder.Name}(value)");
    }
}
