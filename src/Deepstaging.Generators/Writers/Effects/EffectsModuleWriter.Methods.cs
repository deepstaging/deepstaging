// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Effects;

using LocalRefs;
using static LanguageExtRefs;

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
                        .AsStatic()
                        .AddTypeParameter("RT", tp => tp.WithConstraint(module.Capability.Interface))
                        .AddMethodParameters(method)
                        .WithReturnType(EffRT.Of(method.EffReturnType()))
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

        var expression = method.LiftingStrategy switch
        {
            EffectLiftingStrategy.AsyncValue => lift.Async(method.EffResultType, methodCall),
            EffectLiftingStrategy.AsyncVoid => lift.AsyncVoid(methodCall),
            EffectLiftingStrategy.AsyncNullableToOption => lift.AsyncOptional(method.EffResultType, methodCall),
            EffectLiftingStrategy.SyncValue => lift.Sync(method.EffResultType, methodCall),
            EffectLiftingStrategy.SyncVoid => lift.SyncVoid(methodCall),
            EffectLiftingStrategy.SyncNullableToOption => lift.SyncOptional(method.EffResultType, methodCall),
            _ => throw new ArgumentOutOfRangeException(nameof(method.LiftingStrategy))
        };

        return module.Instrumented
            ? $"{expression}.WithActivity(\"{module.Name}.{method.EffectName}\", ActivitySource)"
            : expression;
    }

    private static TypeRef EffReturnType(this EffectMethodModel method) => method.LiftingStrategy switch
    {
        EffectLiftingStrategy.AsyncNullableToOption or EffectLiftingStrategy.SyncNullableToOption =>
            Option(method.EffResultType),
        _ => method.EffResultType
    };
}