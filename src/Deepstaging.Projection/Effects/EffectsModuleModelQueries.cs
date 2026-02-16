// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using static Deepstaging.Projection.Effects.Models.EffectLiftingStrategy;

namespace Deepstaging.Projection.Effects;

using Attributes;
using Models;

/// <summary>
/// Extension methods for building effects module models from symbols decorated with <see cref="EffectsModuleAttribute"/>.
/// </summary>
public static class EffectsModuleModelQueries
{
    extension(ValidSymbol<INamedTypeSymbol> container)
    {
        /// <summary>
        /// Queries all <see cref="EffectsModuleAttribute"/> instances on this type and builds corresponding models.
        /// Each model contains the effect methods, DbSets (if applicable), and capability information.
        /// </summary>
        /// <returns>An immutable array of <see cref="Models.EffectsModuleModel"/> instances, one per attribute.</returns>
        public ImmutableArray<EffectsModuleModel> QueryEffectsModules()
        {
            return
            [
                ..container
                    .GetAttributes<EffectsModuleAttribute>()
                    .Select(attr =>
                        {
                            var attribute = attr.AsQuery<EffectsModuleAttributeQuery>();

                            return new EffectsModuleModel
                            {
                                Capability = attribute.TargetType.CreateCapabilityModel(),
                                EffectsContainerName = container.Name,
                                Accessibility = attribute.TargetType.AccessibilityString,
                                Methods = attribute.CreateEffectMethods(),
                                Instrumented = attribute.Instrumented,
                                Name = attribute.Name,
                                Namespace = container.Namespace ?? "Global",
                                TargetType = attribute.TargetType.GloballyQualifiedName,
                                TargetTypeName = attribute.TargetType.Name,
                                XmlDocumentation = attribute.TargetType.XmlDocumentation.ToSnapshot(),
                                IsDbContext = attribute.TargetType.IsEfDbContext(),
                                DbSets = attribute.CreateDbSets()
                            };
                        }
                    )
            ];
        }
    }

    private static EquatableArray<EffectMethodModel> CreateEffectMethods(this EffectsModuleAttributeQuery attribute)
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
                        Documentation = method.XmlDocumentation.ToSnapshot(),
                        Parameters =
                        [
                            ..method.Parameters.Select(param => new EffectParameterModel
                                {
                                    Name = param.Name,
                                    Type = param.Type.FullyQualifiedName,
                                    HasDefaultValue = param.HasExplicitDefaultValue,
                                    DefaultValue = param.ExplicitDefaultValue.Map(x => x?.ToString()).OrNull()
                                }
                            )
                        ]
                    };
                }
            );
    }

    private static EquatableArray<DbSetModel> CreateDbSets(this EffectsModuleAttributeQuery attribute)
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
                    }
                )
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
                    "Async methods with return property Task must have a property argument."
                ),
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
            SyncValue => method.ReturnType.GloballyQualifiedName,
            SyncNullableToOption => method.ReturnType.GloballyQualifiedName,

            AsyncValue => method.ReturnType
                .GetFirstTypeArgument()
                .Map(type => type.GloballyQualifiedName)
                .OrThrow("Expected a property argument for async value return property."),

            AsyncNullableToOption => method.ReturnType
                .GetFirstTypeArgument()
                .Map(type => type.GloballyQualifiedName)
                .OrThrow("Expected a property argument for async value return property."),

            _ => "Unit"
        };
    }

    private static RuntimeCapabilityModel CreateCapabilityModel(this ValidSymbol<INamedTypeSymbol> targetType)
    {
        var methods = targetType.QueryMethods().GetAll();

        return new RuntimeCapabilityModel
        {
            Interface = $"IHas{targetType.PropertyName}",
            PropertyName = targetType.PropertyName,
            ParameterName = targetType.ParameterName,
            DependencyType = targetType.ToSnapshot(),
            Methods =
            [
                ..methods.Select(method => new CapabilityMethodModel
                    {
                        Name = method.Name,
                        ReturnType = method.ReturnType.GloballyQualifiedName,
                        ReturnsVoid = method.ReturnsVoid,
                        IsNonGenericTask = method.ReturnType.IsNonGenericTask,
                        IsNonGenericValueTask = method.ReturnType.IsNonGenericValueTask,
                        IsGenericTask = method.ReturnType.IsGenericTask,
                        IsGenericValueTask = method.ReturnType.IsGenericValueTask,
                        InnerTaskType = method.ReturnType.InnerTaskType switch
                        {
                            { IsEmpty: false } inner => inner.GloballyQualifiedName,
                            _ => null
                        },
                        Parameters =
                        [
                            ..method.Parameters.Select(p => new CapabilityParameterModel
                                {
                                    Name = p.Name,
                                    Type = p.Type.GloballyQualifiedName
                                }
                            )
                        ],
                        DelegateType = method.ReturnsVoid
                            ? DelegateRefs.Action([..method.Parameters.Select(p => TypeRef.From(p.Type))])
                            : DelegateRefs.Func([..method.Parameters.Select(p => TypeRef.From(p.Type)), TypeRef.From(method.ReturnType)])
                    }
                )
            ]
        };
    }
}