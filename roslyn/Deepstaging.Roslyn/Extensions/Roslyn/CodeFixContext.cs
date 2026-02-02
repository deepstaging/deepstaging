using Microsoft.CodeAnalysis.CodeFixes;

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for CodeFixContext - provides direct access to the query and projection system.
/// </summary>
public static class CodeFixContextExtensions
{
    extension(CodeFixContext context)
    {
        /// <summary>
        /// Queries a type symbol from the given syntax node.
        /// </summary>
        public async Task<OptionalSymbol<INamedTypeSymbol>> QueryType(SyntaxNode node,
            CancellationToken cancellationToken = default)
        {
            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            return semanticModel is null
                ? OptionalSymbol<INamedTypeSymbol>.Empty()
                : semanticModel.GetSymbolInfo(node, cancellationToken).Symbol.AsNamedType();
        }

        /// <summary>
        /// Queries a method symbol from the given syntax node.
        /// </summary>
        public async Task<OptionalSymbol<IMethodSymbol>> QueryMethod(SyntaxNode node,
            CancellationToken cancellationToken = default)
        {
            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            return semanticModel is null
                ? OptionalSymbol<IMethodSymbol>.Empty()
                : semanticModel.GetSymbolInfo(node, cancellationToken).Symbol.AsMethod();
        }

        /// <summary>
        /// Queries a property symbol from the given syntax node.
        /// </summary>
        public async Task<OptionalSymbol<IPropertySymbol>> QueryProperty(SyntaxNode node,
            CancellationToken cancellationToken = default)
        {
            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            return semanticModel is null
                ? OptionalSymbol<IPropertySymbol>.Empty()
                : semanticModel.GetSymbolInfo(node, cancellationToken).Symbol.AsProperty();
        }

        /// <summary>
        /// Queries a field symbol from the given syntax node.
        /// </summary>
        public async Task<OptionalSymbol<IFieldSymbol>> QueryField(SyntaxNode node,
            CancellationToken cancellationToken = default)
        {
            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            return semanticModel is null
                ? OptionalSymbol<IFieldSymbol>.Empty()
                : semanticModel.GetSymbolInfo(node, cancellationToken).Symbol.AsField();
        }

        /// <summary>
        /// Gets the symbol from the given syntax node.
        /// </summary>
        public async Task<ISymbol?> GetSymbol(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            return semanticModel?.GetSymbolInfo(node, cancellationToken).Symbol;
        }

        /// <summary>
        /// Gets the declared symbol from the given syntax node.
        /// </summary>
        public async Task<ISymbol?> GetDeclaredSymbol(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            return semanticModel?.GetDeclaredSymbol(node, cancellationToken);
        }
    }
}