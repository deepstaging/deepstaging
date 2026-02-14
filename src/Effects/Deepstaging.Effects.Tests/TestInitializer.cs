// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Runtime.CompilerServices;
using Deepstaging.Effects.Runtime;

namespace Deepstaging.Effects.Tests;

internal static class TestInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(EffectsModuleAttribute),
            typeof(ActivityEffectExtensions),
            typeof(LanguageExt.Seq),
            typeof(Microsoft.Extensions.Logging.ILogger),
            typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection),
            typeof(Microsoft.EntityFrameworkCore.DbContext)
        );
    }
}
