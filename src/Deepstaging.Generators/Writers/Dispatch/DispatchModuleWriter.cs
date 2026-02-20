// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Dispatch;

using Deepstaging.Projection.Dispatch.Models;

/// <summary>
/// Emits the Dispatch module with direct Eff composition methods per command/query.
/// </summary>
public static class DispatchModuleWriter
{
    extension(DispatchModel model)
    {
        /// <summary>
        /// Emits the dispatch module as a static partial class with overloaded dispatch methods.
        /// </summary>
        public OptionalEmit WriteDispatchModule()
        {
            var builder = TypeBuilder
                .Parse($"{model.Accessibility} static partial class {model.ContainerName}")
                .InNamespace(model.Namespace)
                .AddUsings("LanguageExt", "LanguageExt.Effects")
                .WithXmlDoc(xml => xml
                    .WithSummary($"Generated dispatch module with typed dispatch methods.")
                );

            // Emit command dispatch methods
            foreach (var group in model.CommandHandlers)
            {
                foreach (var method in group.Methods)
                {
                    builder = builder.AddMethod(
                        CreateCommandDispatchMethod(model, group, method)
                    );
                }
            }

            // Emit query dispatch methods
            foreach (var group in model.QueryHandlers)
            {
                foreach (var method in group.Methods)
                {
                    builder = builder.AddMethod(
                        CreateQueryDispatchMethod(group, method)
                    );
                }
            }

            return builder.Emit();
        }
    }

    private static MethodBuilder CreateCommandDispatchMethod(
        DispatchModel model,
        DispatchHandlerGroupModel group,
        DispatchHandlerMethodModel method)
    {
        var handlerCall = $"from result in {group.HandlerType}.{method.MethodName}(command)";

        var body = model.AutoCommit
            ? handlerCall
              + $"\nfrom _ in liftEff<{method.RuntimeType}, Unit>(async rt => {{ if (rt is global::Deepstaging.Dispatch.IAutoCommittable c) await c.CommitAsync(default); return unit; }})"
              + "\nselect result"
            : $"{handlerCall}\nselect result";

        return MethodBuilder
            .Parse($"public static Eff<{method.RuntimeType}, {method.ResultType}> Dispatch({method.InputType} command)")
            .WithXmlDoc(xml => xml
                .WithSummary($"Dispatches a <see cref=\"{method.InputTypeName}\"/> command to <see cref=\"{group.HandlerTypeName}.{method.MethodName}\"/>.")
            )
            .WithExpressionBody(body);
    }

    private static MethodBuilder CreateQueryDispatchMethod(
        DispatchHandlerGroupModel group,
        DispatchHandlerMethodModel method)
    {
        return MethodBuilder
            .Parse($"public static Eff<{method.RuntimeType}, {method.ResultType}> Dispatch({method.InputType} query)")
            .WithXmlDoc(xml => xml
                .WithSummary($"Dispatches a <see cref=\"{method.InputTypeName}\"/> query to <see cref=\"{group.HandlerTypeName}.{method.MethodName}\"/>.")
            )
            .WithExpressionBody($"from result in {group.HandlerType}.{method.MethodName}(query) select result");
    }
}
