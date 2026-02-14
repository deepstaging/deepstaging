// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;

namespace Deepstaging.Effects.Tests.Generators.Writers;

public class DbSetQueryWriterTests : RoslynTestBase
{
    [Test]
    public async Task EmitsDbSetQueryHelper()
    {
        var emit = ImmutableArray<EffectsModuleModel>.Empty.WriteDbSetQueryHelper();

        await Assert.That(emit).IsSuccessful();
        await Assert.That(CompilationFor(emit.Code!)).IsSuccessful();
        await Verify(emit.Code);

    }
}
