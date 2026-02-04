using Deepstaging.Projection.Attributes;

namespace Deepstaging.Projection;

/// <summary>
/// Extension methods for querying Deepstaging attributes on symbols.
/// </summary>
public static class Queries
{
    extension(ValidAttribute attribute)
    {
        /// <summary>
        /// Converts a <see cref="ValidAttribute"/> to a <see cref="UsesAttributeQuery"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the attribute is not a <see cref="UsesAttribute"/>.</exception>
        public UsesAttributeQuery QueryUsesAttribute() =>
            attribute.Value.As<UsesAttribute>()
                .Map(attr => new UsesAttributeQuery(attr.Value))
                .OrThrow($"attribute must be a valid {nameof(UsesAttribute)}.");

        /// <summary>
        /// Converts a <see cref="ValidAttribute"/> to an <see cref="EffectsModuleAttributeQuery"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the attribute is not an <see cref="EffectsModuleAttribute"/>.</exception>
        public EffectsModuleAttributeQuery QueryEffectsModuleAttribute() =>
            attribute.Value.As<EffectsModuleAttribute>()
                .Map(attr => new EffectsModuleAttributeQuery(attr.Value))
                .OrThrow($"attribute must be a valid {nameof(EffectsModuleAttribute)}.");
    }

    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Gets all <see cref="EffectsModuleAttribute"/> instances applied to this symbol as queryable wrappers.
        /// </summary>
        /// <returns>An immutable array of <see cref="EffectsModuleAttributeQuery"/> instances.</returns>
        public ImmutableArray<EffectsModuleAttributeQuery> EffectsModuleAttributes() =>
        [
            ..symbol.GetAttributes<EffectsModuleAttribute>()
                .Select(attr => attr.QueryEffectsModuleAttribute())
        ];
        
        /// <summary>
        /// Gets all <see cref="UsesAttribute"/> instances applied to this symbol as queryable wrappers.
        /// </summary>
        /// <returns>An immutable array of <see cref="UsesAttributeQuery"/> instances.</returns>
        public ImmutableArray<UsesAttributeQuery> UsesAttributes() =>
        [
            ..symbol.GetAttributes<UsesAttribute>()
                .Select(attr => attr.QueryUsesAttribute())
        ];
    }
}