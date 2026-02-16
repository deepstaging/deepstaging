// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Ids;

using Projection.Ids.Models;
using Roslyn.Emit.Converters;

internal static class EfCoreValueConverterWriter
{
    /// <summary>
    /// Adds an Entity Framework Core ValueConverter nested class.
    /// </summary>
    internal static TypeBuilder AddEfCoreValueConverterClass(
        this TypeBuilder builder,
        StrongIdModel model,
        PropertyBuilder valueProperty) =>
        builder.WithEfCoreValueConverter(
            model.BackingTypeSnapshot.FullyQualifiedName,
            $"id => id.{valueProperty.Name}",
            $"value => new {builder.Name}(value)");
}
