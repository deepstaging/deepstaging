// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deepstaging.Ids.Tests;

public static class TestConfiguration
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(StrongIdAttribute),
            // Converter dependencies for compilation verification
            typeof(TypeConverter),
            typeof(TypeConverterAttribute),
            typeof(System.Text.Json.JsonSerializer),
            typeof(Newtonsoft.Json.JsonConvert),
            typeof(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter),
            typeof(Dapper.SqlMapper));
    }
}
