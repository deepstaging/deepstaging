using Deepstaging.Data.Attributes;
using Deepstaging.Data.Models;
using static Deepstaging.Data.Models.EffectLiftingStrategy;

namespace Deepstaging.Data;

/// <summary>
/// 
/// </summary>
public static class EffectsModule
{
    extension(ValidSymbol<INamedTypeSymbol> container)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ImmutableArray<EffectsModuleModel> ReadEffectsModules() =>
        [
            ..container
                .GetAttributes<EffectsModuleAttribute>()
                .Select(attr =>
                {
                    var attribute = attr.Value.AsEffectsModuleAttribute()
                        .OrThrow("Effects modules must have an EffectsModuleAttribute.");

                    return new EffectsModuleModel
                    {
                        ClassName = container.Name,
                        Namespace = container.Namespace ?? "Global",
                        Name = attribute.Name,
                        Accessibility = attribute.TargetType.AccessibilityString,
                        PropertyName = attribute.TargetType.PropertyName,
                        TargetType = attribute.TargetType.GloballyQualifiedName,
                        TargetTypeName = attribute.TargetType.Name,
                        CapabilityInterface = $"IHas{attribute.TargetType.PropertyName}",
                        Methods = attribute.CreateEffectMethods(),
                        Instrumented = attribute.Instrumented,
                        IsDbContext = attribute.TargetType.IsEntityFrameworkDbContext(),
                        DbSets = attribute.CreateDbSets(),
                    };
                })
        ];
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
                            Type = param.Type.FullyQualifiedName,
                            HasDefaultValue = param.HasExplicitDefaultValue,
                            DefaultValue = param.ExplicitDefaultValue.Map(x => x?.ToString()).OrNull()
                        })
                    ]
                };
            });
    }

    private static ImmutableArray<DbSetModel> CreateDbSets(this EffectsModuleAttributeQuery attribute) =>
        attribute.TargetType
            .QueryProperties()
            .Where(x => x.IsEntityFrameworkDbSet())
            .Select(property =>
            {
                var entity = property.ReturnType.GetFirstTypeArgument()
                    .Map(entity => (Type: entity.GloballyQualifiedName, entity.Name))
                    .OrThrow("Expected a property argument for async value return property.");

                return new DbSetModel
                {
                    PropertyName = property.PropertyName,
                    EntityType = entity.Type,
                    EntityTypeName = entity.Name
                };
            });


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
                { IsNullable: false } => AsyncValue,
            },
            AsyncMethodKind.NotAsync => method.ReturnType.GetFirstTypeArgument() switch
            {
                { IsEmpty: true } => method.ReturnsVoid ? SyncVoid :
                    method.ReturnType.IsNullable ? SyncNullableToOption : SyncValue,
                { IsNullable: true } => SyncNullableToOption,
                { IsNullable: false } => SyncValue,
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