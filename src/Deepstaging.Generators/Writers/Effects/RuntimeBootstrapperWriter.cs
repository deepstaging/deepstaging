// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Effects;

/// <summary>
/// Generates the bootstrapper class with a DI extension method and options for the runtime.
/// </summary>
public static class RuntimeBootstrapperWriter
{
    extension(RuntimeModel model)
    {
        /// <summary>
        /// Generates a bootstrapper class with a DI extension method to register the runtime.
        /// </summary>
        public OptionalEmit WriteRuntimeBootstrapperClass()
        {
            var optionsType = TypeRef.From($"{model.RuntimeTypeName}Options");

            return TypeBuilder.Parse($"public static class {model.RuntimeTypeName}Bootstrapper")
                .InNamespace(model.Namespace)
                .WithXmlDoc($"Provides dependency injection bootstrapping for the <c>{model.RuntimeTypeName}</c> runtime.")
                .AddNestedType(model.OptionsClass(optionsType))
                .AddMethod(model.BootstrapMethod(optionsType))
                .Emit();
        }
    }

    private static TypeBuilder OptionsClass(this RuntimeModel model, TypeRef optionsType) => TypeBuilder
        .Parse($"public sealed class {optionsType}")
        .WithXmlDoc($"Configuration options for bootstrapping the <c>{model.RuntimeTypeName}</c> runtime.")
        .AddTracingOption(optionsType)
        .AddMetricsOption(model, optionsType)
        .AddLoggingOption(model, optionsType);

    private static TypeBuilder AddTracingOption(this TypeBuilder builder, TypeRef optionsType) => builder
        .AddProperty("Tracing", "bool", prop => prop
            .WithAutoPropertyAccessors()
            .WithXmlDoc("Gets or sets whether OpenTelemetry tracing is enabled for effect instrumentation."))
        .AddMethod(MethodBuilder
            .For("EnableTracing")
            .WithReturnType(optionsType)
            .WithXmlDoc(xml => xml
                .WithSummary("Enables OpenTelemetry tracing for instrumented effect methods.")
                .WithReturns("This options instance for fluent chaining."))
            .WithBody(body => body
                .AddStatement("Tracing = true")
                .AddReturn("this")));

    private static TypeBuilder AddLoggingOption(this TypeBuilder builder, RuntimeModel model, TypeRef optionsType) => builder
        .If(model.HasInstrumentedModules, b => b
            .AddProperty("Logging", "bool", prop => prop
                .WithAutoPropertyAccessors()
                .WithXmlDoc("Gets or sets whether structured logging is enabled for effect instrumentation."))
            .AddMethod(MethodBuilder
                .For("EnableLogging")
                .WithReturnType(optionsType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Enables structured logging for instrumented effect methods.")
                    .WithReturns("This options instance for fluent chaining."))
                .WithBody(body => body
                    .AddStatement("Logging = true")
                    .AddReturn("this"))));

    private static TypeBuilder AddMetricsOption(this TypeBuilder builder, RuntimeModel model, TypeRef optionsType) => builder
        .If(model.HasInstrumentedModules, b => b
            .AddProperty("Metrics", "bool", prop => prop
                .WithAutoPropertyAccessors()
                .WithXmlDoc("Gets or sets whether OpenTelemetry metrics are enabled for effect operations."))
            .AddMethod(MethodBuilder
                .For("EnableMetrics")
                .WithReturnType(optionsType)
                .WithXmlDoc(xml => xml
                    .WithSummary("Enables OpenTelemetry metrics for effect operations.")
                    .WithReturns("This options instance for fluent chaining."))
                .WithBody(body => body
                    .AddStatement("Metrics = true")
                    .AddReturn("this"))));

    private static MethodBuilder BootstrapMethod(this RuntimeModel model, TypeRef optionsType)
    {
        var configureDelegate = DelegateTypes.Action(optionsType);

        return MethodBuilder
            .Parse(
                $"""
                 public static {DependencyInjectionTypes.IServiceCollection} Add{model.RuntimeTypeName}(
                     this {DependencyInjectionTypes.IServiceCollection} services, 
                     {configureDelegate.Nullable()} configure = null
                 )
                 """)
            .AddUsings(TaskTypes.Namespace, DependencyInjectionTypes.Namespace, "OpenTelemetry.Trace", "OpenTelemetry.Metrics")
            .WithBody(body => body
                .AddStatement($"var options = new {optionsType}()")
                .AddStatement("configure?.Invoke(options)")
                .ConfigureModuleRegistrations(model)
                .ConfigureRuntimeRegistration(model)
                .ConfigureTracingRegistration(model.ActivitySources)
                .ConfigureMetricsRegistration(model)
                .AddReturn("services"))
            .WithXmlDoc(xml => xml
                .WithSummary($"Registers the <c>{model.RuntimeTypeName}</c> runtime and its dependencies with the service collection.")
                .AddParam("services", "The service collection to add the runtime to.")
                .AddParam("configure", "An optional delegate to configure runtime options such as tracing and logging.")
                .WithReturns("The service collection for fluent chaining."));
    }

    private static BodyBuilder ConfigureTracingRegistration(this BodyBuilder builder, EquatableArray<string> activitySources) => builder
        .If(activitySources is { Count: > 0 }, body => body
            .AddIf("options.Tracing", b =>
            {
                var addSourceCalls = string.Join("", activitySources.Select(source => $".AddSource(\"{source}\")"));
                return b.AddStatement($"services.AddOpenTelemetry().WithTracing(tracing => tracing{addSourceCalls})");
            }));

    private static BodyBuilder ConfigureMetricsRegistration(this BodyBuilder builder, RuntimeModel model) => builder
        .If(model.HasInstrumentedModules, body => body
            .AddIf("options.Metrics", b => b
                .AddStatement($"services.AddSingleton(new global::Deepstaging.Effects.EffectMetrics(\"{model.Namespace}.{model.RuntimeTypeName}\"))")
                .AddStatement("services.AddOpenTelemetry().WithMetrics(metrics => metrics.AddMeter(\"Deepstaging.Effects\"))")));

    private static BodyBuilder ConfigureRuntimeRegistration(this BodyBuilder builder, RuntimeModel model)
    {
        var dependencies = model.Capabilities
            .Select(c => $"var {c.ParameterName} = sp.GetRequiredService<{c.DependencyType.CodeName}>();")
            .ToList();

        var capabilities = model.Capabilities
            .Select(c => c.ParameterName)
            .ToList();

        if (model.HasInstrumentedModules)
        {
            dependencies.Add($"var loggerFactory = options.Logging ? sp.GetRequiredService<{LoggingTypes.ILoggerFactory}>() : null;");
            capabilities.Add("loggerFactory");
        }

        return builder.AddStatement(
            $$"""
              services.AddScoped(sp =>
              {
                  {{string.Join("\n    ", dependencies)}}
                  return new {{model.RuntimeType}}({{string.Join(", ", capabilities)}});
              })
              """
        );
    }

    private static BodyBuilder ConfigureModuleRegistrations(this BodyBuilder builder, RuntimeModel model)
    {
        foreach (var registration in model.Registrations)
        {
            if (registration.AdditionalParameters is { Count: > 0 })
            {
                var paramNames = string.Join(", ", registration.AdditionalParameters.Select(p =>
                    $"options.{registration.MethodName}_{p.Name}"));
                builder = builder.AddStatement($"{registration.ContainingType}.{registration.MethodName}(services, {paramNames})");
            }
            else
            {
                builder = builder.AddStatement($"{registration.ContainingType}.{registration.MethodName}(services)");
            }
        }

        return builder;
    }
}