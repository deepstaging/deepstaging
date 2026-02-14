// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using static Deepstaging.Effects.Generators.Writers.Types;

namespace Deepstaging.Effects.Generators.Writers;

/// <summary>
/// Vanilla effect method emissions (non-DbContext effect methods).
/// </summary>
public static partial class EffectsModuleWriter
{
    extension(TypeBuilder builder)
    {
        private TypeBuilder AddEffectMethods(EffectsModuleModel module) =>
            builder.If(module.Methods.Any(), b => b
                    .AddUsing("LanguageExt")
                    .AddUsing("LanguageExt.Effects")
                    .AddUsing("static LanguageExt.Prelude"))
                .WithEach(module.Methods, (b, method) => b
                    .AddMethod(method.EffectName, m => m
                        .AsStatic()
                        .AddTypeParameter("RT", tp => tp.WithConstraint(module.Capability.Interface))
                        .AddMethodParameters(method)
                        .WithReturnType(Eff("RT", method.EffResultType))
                        .WithXmlDoc(method.XmlDocumentation)
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

        var expression = method.LiftingStrategy switch
        {
            // liftEff<RT, T>(async rt => await rt.Prop.Method(params))
            EffectLiftingStrategy.AsyncValue =>
                $"liftEff<RT, {method.EffResultType}>(async rt => await {methodCall})",

            // liftEff<RT, Unit>(async rt => { await rt.Prop.Method(params); return unit; })
            EffectLiftingStrategy.AsyncVoid =>
                $$"""liftEff<RT, Unit>(async rt => { await {{methodCall}}; return unit; })""",

            // liftEff<RT, Option<T>>(async rt => Optional(await rt.Prop.Method(params)))
            EffectLiftingStrategy.AsyncNullableToOption =>
                $"liftEff<RT, {method.EffResultType}>(async rt => Optional(await {methodCall}))",

            // liftEff<RT, T>(rt => rt.Prop.Method(params))
            EffectLiftingStrategy.SyncValue =>
                $"liftEff<RT, {method.EffResultType}>(rt => {methodCall})",

            // liftEff<RT, Unit>(rt => { rt.Prop.Method(params); return unit; })
            EffectLiftingStrategy.SyncVoid =>
                $$"""liftEff<RT, Unit>(rt => { {{methodCall}}; return unit; })""",

            // liftEff<RT, Option<T>>(rt => Optional(rt.Prop.Method(params)))
            EffectLiftingStrategy.SyncNullableToOption =>
                $"liftEff<RT, {method.EffResultType}>(rt => Optional({methodCall}))",

            _ => throw new ArgumentOutOfRangeException(nameof(method.LiftingStrategy))
        };

        return module.Instrumented
            ? $"{expression}.WithActivity(\"{module.Name}.{method.EffectName}\", ActivitySource)"
            : expression;
    }
}
