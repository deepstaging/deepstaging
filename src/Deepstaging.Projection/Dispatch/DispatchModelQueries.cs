// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Dispatch;

using Attributes;
using Deepstaging.Dispatch;
using Models;

/// <summary>
/// Extension methods for building dispatch models from symbols decorated with <see cref="DispatchModuleAttribute"/>.
/// </summary>
public static class DispatchModelQueries
{
    extension(ValidSymbol<INamedTypeSymbol> container)
    {
        /// <summary>
        /// Queries the <see cref="DispatchModuleAttribute"/> on this type and builds the corresponding model.
        /// Does not populate handler groups — those are discovered separately via assembly scan.
        /// </summary>
        public DispatchModel? QueryDispatchModule()
        {
            var attributes = container.GetAttributes<DispatchModuleAttribute>();
            if (!attributes.Any()) return null;

            var attribute = attributes.First().AsQuery<DispatchModuleAttributeQuery>();

            return new DispatchModel
            {
                ContainerName = container.Name,
                Namespace = container.Namespace ?? "Global",
                Accessibility = container.AccessibilityString,
                AutoCommit = attribute.AutoCommit
            };
        }
    }

    extension(ValidSymbol<INamedTypeSymbol> handlerClass)
    {
        /// <summary>
        /// Queries this type for command handler methods and builds a handler group model.
        /// Handler methods must be static and have at least one parameter (the command type).
        /// </summary>
        public DispatchHandlerGroupModel? QueryCommandHandlerGroup()
        {
            if (!handlerClass.GetAttributes<CommandHandlerAttribute>().Any()) return null;

            var methods = handlerClass
                .QueryMethods()
                .Where(m => m.IsStatic && m.Parameters.Length >= 1)
                .Select(method => CreateHandlerMethod(method));

            return new DispatchHandlerGroupModel
            {
                HandlerType = handlerClass.GloballyQualifiedName,
                HandlerTypeName = handlerClass.Name,
                Methods = [..methods]
            };
        }

        /// <summary>
        /// Queries this type for query handler methods and builds a handler group model.
        /// Handler methods must be static and have at least one parameter (the query type).
        /// </summary>
        public DispatchHandlerGroupModel? QueryQueryHandlerGroup()
        {
            if (!handlerClass.GetAttributes<QueryHandlerAttribute>().Any()) return null;

            var methods = handlerClass
                .QueryMethods()
                .Where(m => m.IsStatic && m.Parameters.Length >= 1)
                .Select(method => CreateHandlerMethod(method));

            return new DispatchHandlerGroupModel
            {
                HandlerType = handlerClass.GloballyQualifiedName,
                HandlerTypeName = handlerClass.Name,
                Methods = [..methods]
            };
        }
    }

    private static DispatchHandlerMethodModel CreateHandlerMethod(ValidSymbol<IMethodSymbol> method)
    {
        // Return type is Eff<RT, T> — extract RT and T from type arguments
        var returnType = method.Value.ReturnType;
        var namedReturnType = returnType as INamedTypeSymbol;
        var typeArgs = namedReturnType?.TypeArguments ?? ImmutableArray<ITypeSymbol>.Empty;

        var runtimeType = typeArgs.Length > 0
            ? typeArgs[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            : "RT";
        var resultTypeSymbol = typeArgs.Length > 1
            ? typeArgs[1]
            : returnType;

        return new DispatchHandlerMethodModel
        {
            MethodName = method.Name,
            InputType = method.Parameters[0].Type.GloballyQualifiedName,
            InputTypeName = method.Parameters[0].Type.Name,
            ResultType = resultTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            ResultTypeName = resultTypeSymbol.Name,
            RuntimeType = runtimeType
        };
    }
}
