# Deepstaging.Roslyn.Testing

Test utilities for Roslyn analyzers, generators, and code fixes.

> **See also:** [RoslynTestBase](ROSLYN_TEST_BASE.md) | [Reference Configuration](REFERENCE_CONFIGURATION.md) | [Template Testing](TEMPLATE_TEST_BASE.md) | [Roslyn Toolkit](../Deepstaging.Roslyn/README.md)

## Quick Start

All tests inherit from `RoslynTestBase`:

```csharp
public class MyTests : RoslynTestBase
{
    [Test]
    public async Task TestSymbols()
    {
        var type = SymbolsFor("public class Foo { }").GetType("Foo");
        await Assert.That(type.Name).IsEqualTo("Foo");
    }

    [Test]
    public async Task TestAnalyzer()
    {
        await AnalyzeWith<MyAnalyzer>(source)
            .ShouldReportDiagnostic("MY001");
    }

    [Test]
    public async Task TestGenerator()
    {
        await GenerateWith<MyGenerator>(source)
            .ShouldGenerate()
            .VerifySnapshot();
    }
}
```

### Reference Configuration

If tests need your own assemblies, configure once via `ModuleInitializer`:

```csharp
[ModuleInitializer]
public static void Init() =>
    ReferenceConfiguration.AddReferencesFromTypes(typeof(MyAttribute));
```

---

## API Reference

### RoslynTestBase Methods

| Method | Description |
|--------|-------------|
| `SymbolsFor(source)` | Create compilation and query symbols |
| `CompilationFor(source)` | Get the compilation for source code |
| `AnalyzeWith<T>(source)` | Run analyzer and assert diagnostics |
| `GenerateWith<T>(source)` | Run generator and assert output |
| `FixWith<T>(source)` | Run code fix and assert transformation |
| `AnalyzeAndFixWith<TAnalyzer, TCodeFix>(source)` | Run analyzer then code fix |

### Symbol Testing

```csharp
// Get a type
var type = SymbolsFor(source).GetType("MyClass");

// Query members
var methods = type.QueryMethods().ThatArePublic().GetAll();
var props = type.QueryProperties().ThatAreRequired().GetAll();

// Custom projection
var model = SymbolsFor(source).Project("MyClass", t => new MyModel(t));
```

### Analyzer Testing

```csharp
await AnalyzeWith<MyAnalyzer>(source)
    .ShouldReportDiagnostic("MY001")
    .WithSeverity(DiagnosticSeverity.Error)
    .WithMessage("*must be partial*");

await AnalyzeWith<MyAnalyzer>(source)
    .ShouldNotReportDiagnostic("MY001");
```

### Generator Testing

```csharp
await GenerateWith<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileCount(2)
    .WithFileNamed("Foo.g.cs")
    .WithFileContaining("public partial class")
    .VerifySnapshot();

await GenerateWith<MyGenerator>(source)
    .ShouldNotGenerate();
```

### Code Fix Testing

```csharp
await FixWith<MyCodeFix>(source)
    .ShouldFixTo(expectedSource);

// Or with an analyzer that produces the diagnostics
await AnalyzeAndFixWith<MyAnalyzer, MyCodeFix>(source)
    .ShouldFixTo(expectedSource);
```

---

## Related Documentation

- **[RoslynTestBase](ROSLYN_TEST_BASE.md)** - Full unified API details
- **[Reference Configuration](REFERENCE_CONFIGURATION.md)** - Assembly reference setup
- **[Template Testing](TEMPLATE_TEST_BASE.md)** - Scriban template testing
- **[Main README](../../README.md)** - Project overview
