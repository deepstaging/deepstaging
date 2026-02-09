// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Deepstaging.Analyzers;

namespace Deepstaging.Tests.Analyzers;

public class EffectsModuleIncludeOnlyMethodNotFoundAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenIncludeOnlyMethodDoesNotExist()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService
            {
                void SendAsync();
            }

            [Deepstaging.EffectsModule(typeof(IEmailService), IncludeOnly = ["NonExistentMethod"])]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleIncludeOnlyMethodNotFoundAnalyzer>(source)
            .ShouldReportDiagnostic("DS0007")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*NonExistentMethod*IEmailService*");
    }

    [Test]
    public async Task NoDiagnostic_WhenIncludeOnlyMethodExists()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService
            {
                void SendAsync();
                void Ping();
            }

            [Deepstaging.EffectsModule(typeof(IEmailService), IncludeOnly = ["SendAsync"])]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleIncludeOnlyMethodNotFoundAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoIncludeOnlySpecified()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService
            {
                void SendAsync();
            }

            [Deepstaging.EffectsModule(typeof(IEmailService))]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleIncludeOnlyMethodNotFoundAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoEffectsModule()
    {
        const string source = """
            namespace TestApp;

            public sealed partial class RegularClass;
            """;

        await AnalyzeWith<EffectsModuleIncludeOnlyMethodNotFoundAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsDiagnostic_WhenOneOfMultipleIncludeOnlyMethodsDoesNotExist()
    {
        const string source = """
            namespace TestApp;

            public interface IEmailService
            {
                void SendAsync();
                void ValidateAsync();
            }

            [Deepstaging.EffectsModule(typeof(IEmailService), IncludeOnly = ["SendAsync", "DoesNotExist"])]
            public sealed partial class EmailEffects;
            """;

        await AnalyzeWith<EffectsModuleIncludeOnlyMethodNotFoundAnalyzer>(source)
            .ShouldReportDiagnostic("DS0007")
            .WithMessage("*DoesNotExist*IEmailService*");
    }
}
