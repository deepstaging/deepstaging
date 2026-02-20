// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.EventQueue;

public class EventQueueShouldBeStaticAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotStatic()
    {
        const string source =
            """
            using Deepstaging.EventQueue;

            namespace TestApp;

            [EventQueue("DomainEvents")]
            public partial class DomainEvents;
            """;

        await AnalyzeWith<EventQueueShouldBeStaticAnalyzer>(source)
            .ShouldReportDiagnostic("DSEQ02")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
            .WithMessage("*DomainEvents*static*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsStatic()
    {
        const string source =
            """
            using Deepstaging.EventQueue;

            namespace TestApp;

            [EventQueue("DomainEvents")]
            public static partial class DomainEvents;
            """;

        await AnalyzeWith<EventQueueShouldBeStaticAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
