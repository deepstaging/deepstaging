// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Projection.Attributes;
using Deepstaging.Projection.Models;
using static Deepstaging.Projection.Models.EffectLiftingStrategy;

namespace Deepstaging.Projection;

/// <summary>
/// Extension methods for building effects module models from symbols decorated with <see cref="EffectsModuleAttribute"/>.
/// </summary>
public static class EffectsModule
{
    extension(ValidSymbol<INamedTypeSymbol> container)
    {
        /// <summary>
        /// Queries all <see cref="EffectsModuleAttribute"/> instances on this type and builds corresponding models.
        /// Each model contains the effect methods, DbSets (if applicable), and capability information.
        /// </summary>
        /// <returns>An immutable array of <see cref="EffectsModuleModel"/> instances, one per attribute.</returns>
        public ImmutableArray<EffectsModuleModel> QueryEffectsModules()
        {
            return
            [
                ..container
                    .GetAttributes<EffectsModuleAttribute>()
                    .Select(attr =>
                    {
                        var attribute = attr.QueryEffectsModuleAttribute();

                        return new EffectsModuleModel
                        {
                            Capability = new RuntimeCapabilityModel
                            {
                                Interface = $"IHas{attribute.TargetType.PropertyName}",
                                PropertyName = attribute.TargetType.PropertyName,
                                ParameterName = attribute.TargetType.ParameterName,
                                DependencyType = attribute.TargetType.GloballyQualifiedName
                            },
                            EffectsContainerName = container.Name,
                            Accessibility = attribute.TargetType.AccessibilityString,
                            Methods = attribute.CreateEffectMethods(),
                            Instrumented = attribute.Instrumented,
                            Name = attribute.Name,
                            Namespace = container.Namespace ?? "Global",
                            TargetType = attribute.TargetType.GloballyQualifiedName,
                            TargetTypeName = attribute.TargetType.Name,
                            XmlDocumentation = attribute.TargetType.XmlDocumentation,
                            IsDbContext = attribute.TargetType.IsEfDbContext(),
                            DbSets = attribute.CreateDbSets(),
                        };
                    })
            ];
        }
    }

    private static ImmutableArray<EffectMethodModel> CreateEffectMethods(this EffectsModuleAttributeQuery attribute)
    {
        var exclude = attribute.Exclude.IsDefaultOrEmpty ? null : new HashSet<string>(attribute.Exclude);
        var includeOnly = attribute.IncludeOnly.IsDefaultOrEmpty ? null : new HashSet<string>(attribute.IncludeOnly);

        return attribute.TargetType
            .QueryMethods()
            .Where(method => exclude == null || !exclude.Contains(method.Name))
            .Where(method => includeOnly == null || includeOnly.Contains(method.Name))
            .Select(static method =>
            {
                var liftingStrategy = method.DetermineLiftingStrategy();

                return new EffectMethodModel
                {
                    EffectName = method.Name,
                    SourceMethodName = method.Name,
                    EffResultType = method.EffectResultType(liftingStrategy),
                    LiftingStrategy = liftingStrategy,
                    XmlDocumentation = method.XmlDocumentation,
                    Parameters =
                    [
                        ..method.Parameters.Select(param => new EffectParameterModel
                        {
                            Name = param.Name,
                            Type = param.Type?.FullyQualifiedName!,
                            HasDefaultValue = param.HasExplicitDefaultValue,
                            DefaultValue = param.ExplicitDefaultValue.Map(x => x?.ToString()).OrNull()
                        })
                    ]
                };
            });
    }

    private static ImmutableArray<DbSetModel> CreateDbSets(this EffectsModuleAttributeQuery attribute)
    {
        return attribute.TargetType
            .QueryProperties()
            .Where(x => x.IsEfDbSet())
            .Select(property => property
                .ReturnType
                .GetFirstTypeArgument()
                .Map(entity => new DbSetModel
                {
                    PropertyName = property.PropertyName,
                    EntityType = entity.GloballyQualifiedName,
                    EntityTypeName = entity.Name
                })
                .OrThrow($"Expected {property.Name} to return an entity type.")
            );
    }


    private static EffectLiftingStrategy DetermineLiftingStrategy(this ValidSymbol<IMethodSymbol> method)
    {
        return method.AsyncKind switch
        {
            AsyncMethodKind.Void => AsyncVoid,
            AsyncMethodKind.Value => method.ReturnType.InnerTaskType switch
            {
                { IsEmpty: true } => throw new InvalidOperationException(
                    "Async methods with return property Task must have a property argument."),
                { IsNullable: true } => AsyncNullableToOption,
                { IsNullable: false } => AsyncValue
            },
            AsyncMethodKind.NotAsync => method.ReturnType.GetFirstTypeArgument() switch
            {
                { IsEmpty: true } => method.ReturnsVoid ? SyncVoid :
                    method.ReturnType.IsNullable ? SyncNullableToOption : SyncValue,
                { IsNullable: true } => SyncNullableToOption,
                { IsNullable: false } => SyncValue
            },
            _ => SyncVoid
        };
    }

    private static string EffectResultType(this ValidSymbol<IMethodSymbol> method, EffectLiftingStrategy strategy)
    {
        return strategy switch
        {
            SyncVoid => "Unit",
            AsyncVoid => "Unit",
            SyncValue => method.ReturnType.FullyQualifiedName,
            SyncNullableToOption => $"Option<{method.ReturnType.FullyQualifiedName}>",

            AsyncValue => method.ReturnType
                .GetFirstTypeArgument()
                .Map(x => x.FullyQualifiedName)
                .OrThrow("Expected a property argument for async value return property."),

            AsyncNullableToOption => method.ReturnType
                .GetFirstTypeArgument()
                .Map(x => $"Option<{x.FullyQualifiedName}>")
                .OrThrow("Expected a property argument for async value return property."),

            _ => "Unit"
        };
    }
}