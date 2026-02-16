// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Effects;

/// <summary>
/// Vanilla effect method emissions (non-DbContext effect methods).
/// </summary>
public static partial class EffectsModuleWriter
{
    extension(TypeBuilder builder)
    {
        private TypeBuilder AddEffectMethods(EffectsModuleModel module) =>
            builder.If(module.Methods.Any(), b => b.AddLanguageExtUsings())
                .WithEach(module.Methods, (b, method) => b
                    .AddMethod(method.EffectName, m => m
                        .AsEffMethod("RT", module.Capability.Interface, method.LiftingStrategy.EffReturnType(method.EffResultType))
                        .AddMethodParameters(method)
                        .WithXmlDoc(method.Documentation)
                        .WithExpressionBody(module.LiftedMethodExpression(method))));
    }

    private static MethodBuilder AddMethodParameters(this MethodBuilder builder, EffectMethodModel method) =>
        builder.WithEach(method.Parameters, (b, param) => param.HasDefaultValue
            ? b.AddParameter(param.Name, param.Type, p => p.WithDefaultValue(param.DefaultValue!))
            : b.AddParameter(param.Name, param.Type));

    private static string LiftedMethodExpression(this EffectsModuleModel module, EffectMethodModel method)
    {
        var paramList = string.Join(", ", method.Parameters.Select(p => p.Name));
        var methodCall = $"rt.{module.Capability.PropertyName}.{method.SourceMethodName}({paramList})";
        var lift = EffExpression.Lift("RT", "rt");

        var expression = lift.Lift(method.LiftingStrategy, method.EffResultType, methodCall);

        return module.Instrumented
            ? $"{expression}.WithActivity(\"{module.Name}.{method.EffectName}\", ActivitySource)"
            : expression;
    }
}