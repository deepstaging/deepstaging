// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.CodeFixes.Ids;

using Deepstaging.Roslyn.Scriban;
using Deepstaging.Roslyn.Testing;

/// <summary>
/// Tests that the scaffold code fix creates a user template file for [StrongId] types.
/// </summary>
public class ScaffoldTemplateCodeFixTests : RoslynTestBase
{
    private const string Source =
        """
        using Deepstaging.Ids;

        [assembly: System.Reflection.AssemblyMetadata(
            "Deepstaging.Scaffold:Deepstaging.Ids/StrongId",
            "Deepstaging.Ids.StrongIdAttribute")]
        [assembly: System.Reflection.AssemblyMetadata(
            "Deepstaging.Scaffold:Deepstaging.Ids/StrongId:Content",
            "// scaffold content for {{ TypeName }}")]

        namespace TestApp;

        [StrongId]
        public partial struct UserId;
        """;

    [Test]
    public async Task CreatesTemplateFile_WithScaffoldContent()
    {
        await AnalyzeAndFixWith<ScaffoldAvailableAnalyzer, ScaffoldTemplateCodeFix>(Source)
            .ForDiagnostic(ScaffoldDiagnostics.ScaffoldAvailable)
            .ShouldAddAdditionalDocument()
            .WithPathContaining("StrongId")
            .WithContentContaining("scaffold content for {{ TypeName }}");
    }

    [Test]
    public async Task NoFix_WhenUserTemplateExists()
    {
        await AnalyzeWith<ScaffoldAvailableAnalyzer>(Source)
            .WithAdditionalText("Templates/Deepstaging.Ids/StrongId.scriban-cs", "// user template")
            .ShouldHaveNoDiagnostics();
    }
}