// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.EventQueue;

public class EventQueueMustBePartialAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenClassIsNotPartial()
    {
        const string source =
            """
            using Deepstaging.EventQueue;

            namespace TestApp;

            [EventQueue("DomainEvents")]
            public static class DomainEvents;
            """;

        await AnalyzeWith<EventQueueMustBePartialAnalyzer>(source)
            .ShouldReportDiagnostic("DSEQ01")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*DomainEvents*partial*");
    }

    [Test]
    public async Task NoDiagnostic_WhenClassIsPartial()
    {
        const string source =
            """
            using Deepstaging.EventQueue;

            namespace TestApp;

            [EventQueue("DomainEvents")]
            public static partial class DomainEvents;
            """;

        await AnalyzeWith<EventQueueMustBePartialAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
