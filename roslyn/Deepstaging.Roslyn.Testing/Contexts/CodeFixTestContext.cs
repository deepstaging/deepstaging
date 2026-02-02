using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
// ReSharper disable MemberCanBePrivate.Global

namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Provides fluent API for testing code fixes.
/// </summary>
public class CodeFixTestContext
{
    private readonly string _source;
    private readonly CodeFixProvider _codeFix;
    
    internal CodeFixTestContext(string source, CodeFixProvider codeFix)
    {
        _source = source;
        _codeFix = codeFix;
    }
    
    /// <summary>
    /// Specify which diagnostic ID to fix.
    /// </summary>
    /// <param name="diagnosticId">The diagnostic ID that should be fixed.</param>
    /// <returns>A context for making assertions about the fixed code.</returns>
    public CodeFixAssertion ForDiagnostic(string diagnosticId)
    {
        return new CodeFixAssertion(this, diagnosticId);
    }
    
    /// <summary>
    /// Get the code fix provider being tested.
    /// </summary>
    internal CodeFixProvider CodeFix => _codeFix;
    
    /// <summary>
    /// Get the source code being tested.
    /// </summary>
    internal string Source => _source;
}

/// <summary>
/// Represents an assertion about a code fix result.
/// </summary>
public class CodeFixAssertion
{
    private readonly CodeFixTestContext _context;
    private readonly string _diagnosticId;
    
    internal CodeFixAssertion(CodeFixTestContext context, string diagnosticId)
    {
        _context = context;
        _diagnosticId = diagnosticId;
    }
    
    /// <summary>
    /// Assert that applying the code fix produces the expected source code.
    /// </summary>
    /// <param name="expectedSource">The expected source code after applying the fix.</param>
    public async Task ShouldProduce(string expectedSource)
    {
        var compilation = CompilationHelper.CreateCompilation(_context.Source);
        var document = CreateDocument(compilation);
        
        // Get diagnostics for the document
        var semanticModel = await document.GetSemanticModelAsync();
        if (semanticModel == null)
        {
            Assert.Fail("Failed to get semantic model from document");
            return;
        }
        
        var diagnostics = semanticModel.GetDiagnostics();
        var targetDiagnostic = diagnostics.FirstOrDefault(d => d.Id == _diagnosticId);
        
        if (targetDiagnostic == null)
        {
            Assert.Fail($"No diagnostic '{_diagnosticId}' found in source code. Cannot apply fix.");
            return;
        }
        
        // Get code fixes for the diagnostic
        var codeActions = new List<CodeAction>();
        var context = new CodeFixContext(
            document,
            targetDiagnostic,
            (action, _) => codeActions.Add(action),
            CancellationToken.None);
        
        await _context.CodeFix.RegisterCodeFixesAsync(context);
        
        if (codeActions.Count == 0)
        {
            Assert.Fail($"Code fix provider did not register any fixes for diagnostic '{_diagnosticId}'");
            return;
        }
        
        // Apply the first code fix
        var operations = await codeActions[0].GetOperationsAsync(CancellationToken.None);
        var solution = operations
            .OfType<ApplyChangesOperation>()
            .FirstOrDefault()
            ?.ChangedSolution;
        
        if (solution == null)
        {
            Assert.Fail("Code fix did not produce a solution with changes");
            return;
        }
        
        // Get the fixed document
        var fixedDocument = solution.GetDocument(document.Id);
        if (fixedDocument == null)
        {
            Assert.Fail("Could not retrieve fixed document from solution");
            return;
        }
        
        var fixedSourceText = await fixedDocument.GetTextAsync();
        var actualSource = fixedSourceText.ToString();
        
        // Normalize line endings for comparison
        var normalizedActual = NormalizeLineEndings(actualSource);
        var normalizedExpected = NormalizeLineEndings(expectedSource);
        
        if (normalizedActual != normalizedExpected)
        {
            Assert.Fail(
                $"Code fix produced unexpected result.\n\n" +
                $"Expected:\n{normalizedExpected}\n\n" +
                $"Actual:\n{normalizedActual}");
        }
    }
    
    private static Document CreateDocument(Compilation compilation)
    {
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);
        
        var solution = new AdhocWorkspace()
            .CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddDocument(documentId, "TestDocument.cs", SourceText.From(compilation.SyntaxTrees.First().ToString()));
        
        var document = solution.GetDocument(documentId);
        if (document == null)
        {
            throw new InvalidOperationException("Failed to create test document");
        }
        
        return document;
    }
    
    private static string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
    }
}
