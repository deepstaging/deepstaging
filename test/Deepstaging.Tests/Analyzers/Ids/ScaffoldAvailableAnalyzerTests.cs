// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.Analyzers.Ids;

using Deepstaging.Roslyn.Scriban;
using Deepstaging.Roslyn.Testing;

/// <summary>
/// Tests that DSRK005 fires for [StrongId] types without a user template.
/// </summary>
public class ScaffoldAvailableAnalyzerTests : RoslynTestBase
{
    private const string ScaffoldMetadataSource =
        """
        [assembly: System.Reflection.AssemblyMetadata(
            "Deepstaging.Scaffold:Deepstaging.Ids/StrongId",
            "Deepstaging.Ids.StrongIdAttribute")]
        [assembly: System.Reflection.AssemblyMetadata(
            "Deepstaging.Scaffold:Deepstaging.Ids/StrongId:Content",
            "// scaffold content for {{ TypeName }}")]
        """;

    [Test]
    public async Task ReportsDiagnostic_WhenStrongIdType_HasNoTemplate()
    {
        var source =
            $$"""
              using Deepstaging.Ids;

              {{ScaffoldMetadataSource}}

              namespace TestApp;

              [StrongId]
              public partial struct UserId;
              """;

        await AnalyzeWith<ScaffoldAvailableAnalyzer>(source)
            .ShouldReportDiagnostic(ScaffoldDiagnostics.ScaffoldAvailable)
            .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Info)
            .WithMessage("*UserId*Deepstaging.Ids/StrongId*");
    }

    [Test]
    public async Task NoDiagnostic_WhenUserTemplateExists()
    {
        var source =
            $$"""
              using Deepstaging.Ids;

              {{ScaffoldMetadataSource}}

              namespace TestApp;

              [StrongId]
              public partial struct UserId;
              """;

        await AnalyzeWith<ScaffoldAvailableAnalyzer>(source)
            .WithAdditionalText("Templates/Deepstaging.Ids/StrongId.scriban-cs", "// user override")
            .ShouldHaveNoDiagnostics();
    }

    [Test]
    public async Task NoDiagnostic_WhenNoStrongIdAttribute()
    {
        var source =
            $$"""
              {{ScaffoldMetadataSource}}

              namespace TestApp;

              public partial struct PlainStruct;
              """;

        await AnalyzeWith<ScaffoldAvailableAnalyzer>(source)
            .ShouldHaveNoDiagnostics();
    }
}