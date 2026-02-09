// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Comparison;
using Deepstaging.Roslyn.Emit.Interfaces.Equality;
using Deepstaging.Roslyn.Emit.Interfaces.Formatting;
using Deepstaging.Roslyn.Emit.Interfaces.Parsing;
using Deepstaging.Roslyn.Emit.Operators.Conversions;
using Deepstaging.Roslyn.Emit.Patterns;

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
        /// </summary>
        public OptionalEmit WriteStrongId()
        {
            var typeName = model.TypeName;
            var backingType = model.BackingTypeSymbol;

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
                    isCustomExpression: true)
                .WithBackingConversions(backingType, valueProperty)

                // Factory methods and converters
                .AddFactoryMethods(model)
                .AddParseMethods(model)
                .AddConverters(model, valueProperty)
                .Emit();
        }
    }
}