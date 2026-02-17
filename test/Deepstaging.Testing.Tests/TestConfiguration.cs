// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Testing.Tests;

using Effects;

internal static class TestConfiguration
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(EffectsModuleAttribute),
            typeof(TestRuntimeAttribute<>),
            typeof(LanguageExt.Seq)
        );
    }
}
