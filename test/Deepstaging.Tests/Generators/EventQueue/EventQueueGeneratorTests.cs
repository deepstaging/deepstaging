// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Generators.EventQueue;

using Deepstaging.Generators;

public class EventQueueGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task GeneratesEffectModule_ForBasicQueue()
    {
        const string source =
            """
            using Deepstaging.EventQueue;

            namespace TestApp;

            [EventQueue("DomainEvents")]
            public static partial class DomainEvents;
            """;

        await GenerateWith<EventQueueGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("static partial class DomainEvents")
            .WithFileContaining("Enqueue")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }
}
