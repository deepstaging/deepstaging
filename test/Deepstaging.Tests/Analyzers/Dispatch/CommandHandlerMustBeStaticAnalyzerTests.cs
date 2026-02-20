// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Dispatch;

public class CommandHandlerMustBeStaticAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotStatic()
    {
        const string source =
            """
            using Deepstaging.Dispatch;

            namespace TestApp;

            [CommandHandler]
            public class OrderCommands;
            """;

        await AnalyzeWith<CommandHandlerMustBeStaticAnalyzer>(source)
            .ShouldReportDiagnostic("DSDSP03")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*OrderCommands*static*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsStatic()
    {
        const string source =
            """
            using Deepstaging.Dispatch;

            namespace TestApp;

            [CommandHandler]
            public static class OrderCommands;
            """;

        await AnalyzeWith<CommandHandlerMustBeStaticAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
