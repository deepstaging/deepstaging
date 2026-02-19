// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Ids;

using Projection.Ids.Models;

/// <summary>
/// Writer for generating strongly-typed ID struct implementations.
/// Uses Deepstaging.Roslyn TypeBuilder extensions for clean, declarative generation.
/// </summary>
public static class TypedIdWriter
{
    extension(TypedIdModel model)
    {
        /// <summary>
        /// Generates the complete strongly-typed ID struct implementation.
        /// </summary>
        public OptionalEmit WriteTypedId()
        {
            var typeName = model.TypeName;
            var backingType = model.BackingTypeSnapshot;

            var valueProperty = PropertyBuilder
                .Parse($"public {backingType.FullyQualifiedName} Value {{ get; }}");

            return TypeBuilder
                .Parse($"{model.Accessibility} partial struct {typeName}")
                .InNamespace(model.Namespace)

                // Core: Value property and constructor
                .AddProperty(valueProperty)
                .AddConstructor(model)

                // Standard interface implementations via extensions
                .ImplementsIEquatable(backingType, valueProperty)
                .ImplementsIComparable(backingType, valueProperty)
                .ImplementsIFormattable(backingType, valueProperty)
                .ImplementsISpanFormattable(backingType, valueProperty)
                .ImplementsIParsable(backingType)
                .ImplementsISpanParsable(backingType)
                .ImplementsIUtf8SpanFormattable(backingType, valueProperty)
                .ImplementsIUtf8SpanParsable(backingType)

                // ToString and conversions
                .OverridesToString(model.BackingType == BackingType.String
                        ? $"{valueProperty.Name} ?? \"\""
                        : $"{valueProperty.Name}.ToString()",
                    true)
                .WithBackingConversions(backingType, valueProperty)

                // Factory methods and converters
                .AddFactoryMethods(model)
                .AddParseMethod(model)
                .AddConverters(model, valueProperty)
                .Emit();
        }
    }
}
