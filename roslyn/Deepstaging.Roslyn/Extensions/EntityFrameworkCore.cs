namespace Deepstaging.Roslyn;

/// <summary>
/// Entity Framework Core extensions for Roslyn symbols.
/// Provides helpers to identify EF Core types like DbSet&lt;T&gt; and DbContext.
/// </summary>
public static class EntityFrameworkCoreExtensions
{
    private const string EfNamespace = "Microsoft.EntityFrameworkCore";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static bool IsEntityFrameworkDbSet(this IPropertySymbol property) =>
        property.Type is { Name: "DbSet" } &&
        property.Type.ContainingNamespace?.ToDisplayString() == EfNamespace;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsEntityFrameworkDbContext(this INamedTypeSymbol type) =>
        type.Name == "DbContext" &&
        type.ContainingNamespace?.ToDisplayString() == EfNamespace;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsEntityFrameworkDbContext(this ValidSymbol<INamedTypeSymbol> type)
    {
        if (type.Value.IsEntityFrameworkDbContext())
            return true;
        
        if (type.BaseType.IsEmpty)
            return false;
        
        return type.BaseType.Symbol?.IsEntityFrameworkDbContext() ?? false;
    }
}