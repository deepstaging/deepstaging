// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Tests.CodeFixes.Ids;

using Deepstaging.Roslyn.Scriban;
using Deepstaging.Roslyn.Testing;

/// <summary>
/// Tests that the scaffold code fix creates a user template file for [TypedId] types.
/// </summary>
public class ScaffoldTemplateCodeFixTests : RoslynTestBase
{
    private const string Source =
        """
        using Deepstaging.Ids;

        [assembly: System.Reflection.AssemblyMetadata(
            "Deepstaging.Scaffold:Deepstaging.Ids/TypedId",
            "Deepstaging.Ids.TypedIdAttribute")]
        [assembly: System.Reflection.AssemblyMetadata(
            "Deepstaging.Scaffold:Deepstaging.Ids/TypedId:Content",
            "// scaffold content for {{ TypeName }}")]

        namespace TestApp;

        [TypedId]
        public partial struct UserId;
        """;

    [Test]
    public async Task CreatesTemplateFile_WithScaffoldContent()
    {
        await AnalyzeAndFixWith<ScaffoldAvailableAnalyzer, ScaffoldTemplateCodeFix>(Source)
            .ForDiagnostic(ScaffoldDiagnostics.ScaffoldAvailable)
            .ShouldAddAdditionalDocument()
            .WithPathContaining("TypedId")
            .WithContentContaining("scaffold content for {{ TypeName }}");
    }

    [Test]
    public async Task NoFix_WhenUserTemplateExists()
    {
        await AnalyzeWith<ScaffoldAvailableAnalyzer>(Source)
            .WithAdditionalText("Templates/Deepstaging.Ids/TypedId.scriban-cs", "// user template")
            .ShouldHaveNoDiagnostics();
    }
}