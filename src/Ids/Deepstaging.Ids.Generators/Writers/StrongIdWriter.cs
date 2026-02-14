// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Comparison;
using Deepstaging.Roslyn.Emit.Interfaces.Equality;
using Deepstaging.Roslyn.Emit.Interfaces.Formatting;
using Deepstaging.Roslyn.Emit.Interfaces.Parsing;
using Deepstaging.Roslyn.Emit.Operators.Conversions;
using Deepstaging.Roslyn.Emit.Patterns;
using Deepstaging.Roslyn.Scriban;

namespace Deepstaging.Ids.Generators.Writers;

/// <summary>
/// Writer for generating strongly-typed ID struct implementations.
/// Uses Deepstaging.Roslyn TypeBuilder extensions for clean, declarative generation.
/// </summary>
public static class StrongIdWriter
{
    extension(StrongIdModel model)
    {
        /// <summary>
        /// Generates the complete strongly-typed ID struct implementation.
        /// Records explicit template bindings via the provided <paramref name="map"/>.
        /// </summary>
        public OptionalEmit WriteStrongId(TemplateMap<StrongIdModel> map)
        {
            var typeName = map.Bind(model.TypeName, m => m.TypeName);
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
                .AddParseMethods(model)
                .AddConverters(model, valueProperty)
                .Emit();
        }
    }
}
