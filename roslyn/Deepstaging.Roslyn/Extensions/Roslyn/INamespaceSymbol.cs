namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for INamespaceSymbol to query types.
/// </summary>
public static class NamespaceQueryExtensions
{
    extension(INamespaceSymbol namespaceSymbol)
    {
        /// <summary>
        /// Creates a TypeQuery to search for types in the namespace.
        /// </summary>
        public TypeQuery QueryTypes()
        {
            return TypeQuery.From(namespaceSymbol);
        }
    }
}