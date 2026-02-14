// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects.Tests.Analyzers;

public class EffectsModuleExcludeMethodNotFoundAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenExcludedMethodDoesNotExist()
    {
        const string source = """
                              namespace TestApp;

                              public interface IEmailService
                              {
                                  void SendAsync();
                              }

                              [Deepstaging.EffectsModule(typeof(IEmailService), Exclude = ["NonExistentMethod"])]
                              public sealed partial class EmailEffects;
                              """;

        await AnalyzeWith<EffectsModuleExcludeMethodNotFoundAnalyzer>(source)
            .ShouldReportDiagnostic("DS0006")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*NonExistentMethod*IEmailService*");
    }

    [Test]
    public async Task NoDiagnostic_WhenExcludedMethodExists()
    {
        const string source = """
                              namespace TestApp;

                              public interface IEmailService
                              {
                                  void SendAsync();
                                  void Ping();
                              }

                              [Deepstaging.EffectsModule(typeof(IEmailService), Exclude = ["Ping"])]
                              public sealed partial class EmailEffects;
                              """;

        await AnalyzeWith<EffectsModuleExcludeMethodNotFoundAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoExcludeSpecified()
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

        await AnalyzeWith<EffectsModuleExcludeMethodNotFoundAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoEffectsModule()
    {
        const string source = """
                              namespace TestApp;

                              public sealed partial class RegularClass;
                              """;

        await AnalyzeWith<EffectsModuleExcludeMethodNotFoundAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task ReportsDiagnostic_WhenOneOfMultipleExcludedMethodsDoesNotExist()
    {
        const string source = """
                              namespace TestApp;

                              public interface IEmailService
                              {
                                  void SendAsync();
                                  void ValidateAsync();
                              }

                              [Deepstaging.EffectsModule(typeof(IEmailService), Exclude = ["SendAsync", "DoesNotExist"])]
                              public sealed partial class EmailEffects;
                              """;

        await AnalyzeWith<EffectsModuleExcludeMethodNotFoundAnalyzer>(source)
            .ShouldReportDiagnostic("DS0006")
            .WithMessage("*DoesNotExist*IEmailService*");
    }
}
