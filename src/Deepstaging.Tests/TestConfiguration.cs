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
            typeof(Config.ConfigRootAttribute),
            // Ids
            typeof(Ids.StrongIdAttribute),
            typeof(System.ComponentModel.TypeConverter),
            typeof(System.ComponentModel.TypeConverterAttribute),
            typeof(Newtonsoft.Json.JsonConvert),
            typeof(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter),
            typeof(Dapper.SqlMapper),
            // HttpClient
            typeof(HttpClient.HttpClientAttribute),
            typeof(System.Net.Http.HttpClient),
            typeof(System.Text.Json.JsonSerializer)
        );
    }
}