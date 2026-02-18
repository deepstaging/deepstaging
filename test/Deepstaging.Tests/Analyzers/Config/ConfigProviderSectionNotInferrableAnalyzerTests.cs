// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Analyzers.Config;
using Deepstaging.Roslyn.Testing;

namespace Deepstaging.Tests.Analyzers.Config;

/// <summary>
/// Tests for <see cref="ConfigProviderSectionNotInferrableAnalyzer"/>.
/// </summary>
public class ConfigProviderSectionNotInferrableAnalyzerTests : RoslynTestBase
{
    [Test]
    public async Task ReportsDiagnostic_WhenSectionCannotBeInferred()
    {
        const string source = """
            namespace TestApp;
            [Deepstaging.Config.ConfigProvider]
            public partial class MySettings;
        """;

        await AnalyzeWith<ConfigProviderSectionNotInferrableAnalyzer>(source)
            .ShouldReportDiagnostic("DSCFG03")
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .WithMessage("*MySettings*");
    }

    [Test]
    public async Task NoDiagnostic_WhenSectionIsExplicit()
    {
        const string source = """
            namespace TestApp;
            [Deepstaging.Config.ConfigProvider(Section = "MyApp")]
            public partial class MySettings;
        """;

        await AnalyzeWith<ConfigProviderSectionNotInferrableAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenSectionIsInferredFromSuffix()
    {
        const string source = """
            namespace TestApp;
            [Deepstaging.Config.ConfigProvider]
            public partial class SlackConfigProvider;
        """;

        await AnalyzeWith<ConfigProviderSectionNotInferrableAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}
