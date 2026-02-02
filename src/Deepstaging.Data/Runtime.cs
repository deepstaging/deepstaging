using Deepstaging.Data.Models;

namespace Deepstaging.Effects.Queries;

/// <summary>
/// 
/// </summary>
public static class Runtime
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static RuntimeModel? QueryRuntimeModel(this ISymbol symbol)
    {
        return symbol.AsNamedType()
            .Map(CreateModel)
            .OrNull();

        RuntimeModel CreateModel(ValidSymbol<INamedTypeSymbol> runtimeType) => new()
        {
            ClassName = runtimeType.Name,
            Namespace = runtimeType.Namespace ?? "Global",
            Accessibility = runtimeType.Accessibility,
            AccessibilityModifier = runtimeType.AccessibilityString,
            UsedModuleReferences =
            [
                ..runtimeType.GetAllUsesAttributes()
                    .SelectMany(query => query.AllModuleReferences())
            ],
        };
    }

    private static ImmutableArray<UsedModuleReference> AllModuleReferences(this UsesAttributeQuery attribute)
    {
        var references = ImmutableArray.CreateBuilder<UsedModuleReference>();

        attribute.ModuleType.GetEffectsModuleAttribute()
            .Select(attr => attr.ModuleReferenceFor(attribute.ModuleType))
            .Do(reference => references.Add(reference));

        attribute.ModuleType.GetEventQueueModuleAttribute()
            .Select(attr => attr.ModuleReferenceFor(attribute.ModuleType))
            .Do(reference => references.Add(reference));

        attribute.ModuleType.GetDispatcherModuleAttribute()
            .Select(attr => attr.ModuleReferenceFor(attribute.ModuleType))
            .Do(reference => references.Add(reference));

        return references.ToImmutable();
    }
}