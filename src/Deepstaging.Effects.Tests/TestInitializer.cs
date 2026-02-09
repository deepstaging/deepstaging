// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Runtime.CompilerServices;

namespace Deepstaging.Tests;

internal static class TestInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(Deepstaging.EffectsModuleAttribute),
            typeof(Deepstaging.Runtime.ActivityEffectExtensions),
            typeof(LanguageExt.Seq),
            typeof(Microsoft.Extensions.Logging.ILogger),
            typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection),
            typeof(Microsoft.EntityFrameworkCore.DbContext)
        );
    }
}
