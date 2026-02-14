// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Runtime.CompilerServices;

namespace Deepstaging.Effects.Testing.Tests;

internal static class TestInitializer
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
