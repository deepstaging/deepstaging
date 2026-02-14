// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Runtime.CompilerServices;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Config.Tests;

internal static class TestConfiguration
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(ConfigRootAttribute));
    }
}
