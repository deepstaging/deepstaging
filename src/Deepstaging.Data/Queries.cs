using Deepstaging.Data.Attributes;

namespace Deepstaging.Data;

/// <summary>
/// 
/// </summary>
public static class Queries
{
    extension(AttributeData data)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public OptionalValue<EffectsModuleAttributeQuery> AsEffectsModuleAttribute() =>
            data.As<EffectsModuleAttribute>()
                .Map(attr => new EffectsModuleAttributeQuery(attr.Value));
    }
}