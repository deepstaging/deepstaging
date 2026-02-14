# Deepstaging.Tests

Unit tests for Deepstaging analyzers, generators, and projection logic.

## Overview

Comprehensive test suite validating the correctness of source generation, analyzer diagnostics, and semantic model extraction.

## Test Categories

### Analyzer Tests

Verifies diagnostics are reported correctly:

```csharp
[Fact]
public async Task EffectsModule_NotPartial_ReportsDiagnostic()
{
    var code = """
        [EffectsModule(typeof(IService))]
        public class MyModule;
        """;
    
    await VerifyAnalyzerAsync(code, DiagnosticResult.DS0001);
}
```

### Generator Tests

Validates generated source output:

```csharp
[Fact]
public async Task Runtime_GeneratesCapabilityInterface()
{
    var code = """
        [Runtime]
        [Uses(typeof(EmailModule))]
        public partial class AppRuntime;
        """;
    
    await VerifyGeneratorAsync(code, expectedOutput);
}
```

### Projection Tests

Tests semantic model extraction:

```csharp
[Fact]
public void EffectsModule_ExtractsMethods()
{
    var symbol = GetSymbol(code);
    var model = symbol.GetEffectsModuleInfo();
    
    Assert.Contains(model.Methods, m => m.Name == "SendAsync");
}
```

## Running Tests

```bash
dotnet test
```

## Project Structure

```
Deepstaging.Tests/
├── Analyzers/      # Analyzer diagnostic tests
├── Generators/     # Source generator output tests
└── Projection/     # Semantic model extraction tests
```

## Dependencies

- `Deepstaging.Roslyn.Testing` - Test infrastructure for Roslyn components

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Analyzers](../Deepstaging.Analyzers/README.md)** — Analyzer implementations being tested
- **[Generators](../Deepstaging.Generators/README.md)** — Generator implementations being tested
- **[Projection](../Deepstaging.Projection/README.md)** — Projection logic being tested
- **[Deepstaging.Roslyn.Testing](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/README.md)** — Test infrastructure
  - [RoslynTestBase](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/RoslynTestBase.md) — Base class docs
  - [GeneratorTestContext](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/GeneratorTestContext.md) — Generator testing
  - [AnalyzerTestContext](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Testing/Docs/AnalyzerTestContext.md) — Analyzer testing

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
