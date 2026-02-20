// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Generators.Dispatch;

using Deepstaging.Generators;

public class DispatchGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task GeneratesDispatchModule_WithCommandHandler()
    {
        const string source =
            """
            using Deepstaging.Dispatch;
            using LanguageExt;
            using LanguageExt.Effects;

            namespace TestApp;

            public record CreateOrder(string Name) : ICommand;
            public record OrderCreated(string Id);

            [CommandHandler]
            public static class OrderCommands
            {
                public static Eff<AppRuntime, OrderCreated> Handle(CreateOrder cmd) =>
                    SuccessEff(new OrderCreated("123"));
            }

            [DispatchModule]
            public static partial class AppDispatch;

            public class AppRuntime;
            """;

        await GenerateWith<DispatchGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("static partial class AppDispatch")
            .WithFileContaining("Dispatch(")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }
}
