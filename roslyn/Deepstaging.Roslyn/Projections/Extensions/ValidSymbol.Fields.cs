namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for ValidSymbol&lt;IFieldSymbol&gt; exposing IFieldSymbol-specific functionality.
/// </summary>
public static class ValidFieldSymbolExtensions
{
    extension(ValidSymbol<IFieldSymbol> field)
    {
        /// <summary>
        /// Gets the type of the field as a validated projected symbol.
        /// Fields always have a type, so this returns a non-nullable ValidSymbol.
        /// </summary>
        public ValidSymbol<ITypeSymbol> Type =>
            ValidSymbol<ITypeSymbol>.From(@field.Value.Type);

        /// <summary>
        /// Gets the type of the field as a validated projected symbol.
        /// Fields always have a type, so this returns a non-nullable ValidSymbol.
        /// </summary>
        public ValidSymbol<ITypeSymbol> ReturnType =>
            ValidSymbol<ITypeSymbol>.From(@field.Value.Type);
    }
}
