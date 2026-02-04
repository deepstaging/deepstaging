using Deepstaging.Data.Attributes;

namespace Deepstaging.Data;

/// <summary>
/// 
/// </summary>
public static class Projections
{
    extension(ValidAttribute attribute)
    {
        /// <summary>
        /// 
        /// </summary>
        public UsesAttributeQuery QueryUsesAttribute() =>
            attribute.Value.As<UsesAttribute>()
                .Map(attr => new UsesAttributeQuery(attr.Value))
                .OrThrow($"attribute must be a valid {nameof(UsesAttribute)}.");
        
        /// <summary>
        /// 
        /// </summary>
        public EffectsModuleAttributeQuery QueryEffectsModuleAttribute() =>
            attribute.Value.As<EffectsModuleAttribute>()
                .Map(attr => new EffectsModuleAttributeQuery(attr.Value))
                .OrThrow($"attribute must be a valid {nameof(EffectsModuleAttribute)}.");
    }
}