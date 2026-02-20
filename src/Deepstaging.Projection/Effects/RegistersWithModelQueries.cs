// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects;

using Attributes;
using Models;

/// <summary>
/// Extension methods for building <see cref="RegistersWithModel"/> from symbols with <see cref="RegistersWithAttribute"/>.
/// </summary>
public static class RegistersWithModelQueries
{
    extension(ValidSymbol<INamedTypeSymbol> container)
    {
        /// <summary>
        /// Queries the <see cref="RegistersWithAttribute"/> on this type and builds a model if valid.
        /// Returns null if the attribute is not present or the referenced method cannot be resolved.
        /// </summary>
        public RegistersWithModel? QueryRegistersWithModel()
        {
            var query = container.RegistersWithAttribute();
            if (query is null)
                return null;

            var method = query.ResolveMethod(container);
            if (method is null)
                return null;

            // Skip the first parameter (this IServiceCollection/etc)
            var additionalParams = method.Parameters
                .Skip(1)
                .Select(p => new RegistersWithParameterModel
                {
                    Name = p.Name,
                    Type = p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    HasDefaultValue = p.HasExplicitDefaultValue,
                    DefaultValue = p.HasExplicitDefaultValue ? FormatDefaultValue(p) : null
                })
                .ToImmutableArray();

            return new RegistersWithModel
            {
                ContainingType = container.GloballyQualifiedName,
                MethodName = query.MethodName,
                ReturnType = method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                AdditionalParameters = additionalParams
            };
        }
    }

    private static string? FormatDefaultValue(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        return parameter.ExplicitDefaultValue switch
        {
            null => "null",
            string s => $"\"{s}\"",
            bool b => b ? "true" : "false",
            var v => v.ToString()
        };
    }
}
