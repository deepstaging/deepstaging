// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests;

using Effects;

internal static class TestConfiguration
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ReferenceConfiguration.AddReferencesFromTypes(
            // Effects
            typeof(EffectsModuleAttribute),
            typeof(ActivityEffectExtensions),
            typeof(LanguageExt.Seq),
            typeof(Microsoft.Extensions.Logging.ILogger),
            typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection),
            typeof(Microsoft.EntityFrameworkCore.DbContext),
            typeof(OpenTelemetry.OpenTelemetryBuilder),
            typeof(OpenTelemetry.Trace.TracerProvider),
            // Config
            typeof(ConfigProviderAttribute),
            typeof(Microsoft.Extensions.Configuration.UserSecretsConfigurationExtensions),
            // Ids
            typeof(Ids.TypedIdAttribute),
            typeof(System.ComponentModel.TypeConverter),
            typeof(System.ComponentModel.TypeConverterAttribute),
            typeof(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter),
            // HttpClient
            typeof(HttpClient.HttpClientAttribute),
            typeof(System.Net.Http.HttpClient),
            typeof(System.Text.Json.JsonSerializer),
            // EventQueue
            typeof(EventQueue.EventQueueAttribute),
            typeof(EventQueue.EventQueueChannel<>),
            // Dispatch
            typeof(Dispatch.DispatchModuleAttribute),
            typeof(Dispatch.ICommand),
            typeof(Dispatch.IQuery<>)
        );
    }
}