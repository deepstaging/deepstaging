// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Runtime.CompilerServices;

namespace Deepstaging.HttpClient.Tests;

public static class TestConfiguration
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(HttpClientAttribute),
            typeof(System.Net.Http.HttpClient),
            typeof(System.Text.Json.JsonSerializer));
    }
}
