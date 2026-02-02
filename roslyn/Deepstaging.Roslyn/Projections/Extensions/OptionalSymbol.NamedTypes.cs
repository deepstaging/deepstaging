namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for OptionalSymbol&lt;INamedTypeSymbol&gt; exposing INamedTypeSymbol-specific functionality.
/// Note: Most properties (IsDelegate, IsEnum, Arity, etc.) are accessible via .Symbol
/// These extensions focus on complex operations like getting nested types, enum underlying types, etc.
/// </summary>
public static class ProjectedNamedTypeSymbolExtensions
{
    extension(OptionalSymbol<INamedTypeSymbol> type)
    {
        /// <summary>
        /// Gets the underlying type of the enum.
        /// Returns Empty if not an enum or underlying type is null.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> EnumUnderlyingType =>
            type is { HasValue: true, Symbol.EnumUnderlyingType: { } underlyingType }
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(underlyingType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Gets the delegate invoke method (for delegate types).
        /// Returns Empty if not a delegate or invoke method is null.
        /// </summary>
        public OptionalSymbol<IMethodSymbol> DelegateInvokeMethod =>
            type is { HasValue: true, Symbol.DelegateInvokeMethod: not null }
                ? OptionalSymbol<IMethodSymbol>.WithValue(type.Symbol!.DelegateInvokeMethod)
                : OptionalSymbol<IMethodSymbol>.Empty();

        /// <summary>
        /// Gets the native integer underlying type (IntPtr or UIntPtr) for nint/nu int types.
        /// Returns Empty if not a native integer type or underlying type is null.
        /// </summary>
        public OptionalSymbol<INamedTypeSymbol> NativeIntegerUnderlyingType =>
            type is { HasValue: true, Symbol.NativeIntegerUnderlyingType: not null }
                ? OptionalSymbol<INamedTypeSymbol>.WithValue(type.Symbol!.NativeIntegerUnderlyingType)
                : OptionalSymbol<INamedTypeSymbol>.Empty();

        /// <summary>
        /// Checks if the type is partial using syntax analysis.
        /// This requires checking DeclaringSyntaxReferences unlike most other checks.
        /// </summary>
        public bool IsPartialType()
        {
            return type.HasValue && type.Symbol!.IsPartial();
        }
    }
}