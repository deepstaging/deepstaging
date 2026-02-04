# RoslynTestBase - Unified Testing API

> **See also:** [Testing README](README.md) | [Reference Configuration](REFERENCE_CONFIGURATION.md) | [Template Testing](TEMPLATE_TEST_BASE.md) | [Roslyn Toolkit](../Deepstaging.Roslyn/README.md)

## Overview

`RoslynTestBase` provides a single, unified base class for all Roslyn component testing (symbols, analyzers, generators, code fixes). This replaces the previous separate base classes with a cleaner, more consistent API.

## Migration Guide

### Before (Old API - Deprecated)
```csharp
// Symbol tests
public class MyQueryTests : SymbolTestBase
{
    // ...
}

// Analyzer tests
public class MyAnalyzerTests : AnalyzerTestBase<MyAnalyzer>
{
    // ...
}

// Generator tests
public class MyGeneratorTests : GeneratorTestBase<MyGenerator>
{
    // ...
}

// Code fix tests
public class MyCodeFixTests : CodeFixTestBase<MyCodeFix>
{
    // ...
}
```

### After (New API)
```csharp
// All tests use the same base class
public class MyRoslynTests : RoslynTestBase
{
    [Test]
    public void TestSymbols()
    {
        var symbols = Symbols(source);
        // ...
    }

    [Test]
    public void TestAnalyzer()
    {
        var result = Analyze<MyAnalyzer>(source);
        // ...
    }

    [Test]
    public void TestGenerator()
    {
        var result = Generate<MyGenerator>(source);
        // ...
    }

    [Test]
    public void TestCodeFix()
    {
        var result = Fix<MyCodeFix>(source);
        // ...
    }
}
```

## API Methods

### `Symbols(string source)`
Create a symbol test context for testing queries and projections.

```csharp
var context = Symbols(source);
var type = context.RequireNamedType("MyClass");
```

### `Symbols(string source, IEnumerable<MetadataReference> additionalReferences)`
Create a symbol test context with additional assembly references.

```csharp
var refs = new[] { MetadataReference.CreateFromFile(typeof(MyAttribute).Assembly.Location) };
var context = Symbols(source, refs);
```

### `Compilation(string source)`
Get the compilation for source code.

```csharp
var compilation = Compilation(source);
```

### `Analyze<TAnalyzer>(string source)`
Run an analyzer against source code.

```csharp
await Analyze<MyAnalyzer>(source)
    .ShouldHaveDiagnostics()
    .WithErrorCode("MYRT001");
```

### `Generate<TGenerator>(string source)`
Run a generator against source code.

```csharp
await Generate<MyGenerator>(source)
    .ShouldGenerate()
    .CodeContaining("public bool Validate()");
```

### `Fix<TCodeFix>(string source)`
Test a code fix provider.

```csharp
await Fix<MyCodeFix>(source)
    .ShouldFixTo(expectedSource);
```

## Project-Specific Test Base

For projects that need additional assembly references (e.g., attribute assemblies), create a derived test base:

```csharp
public abstract class MyProjectTestBase : RoslynTestBase
{
    protected new SymbolTestContext Symbols(string source)
    {
        var additionalReferences = new[]
        {
            MetadataReference.CreateFromFile(typeof(MyAttribute).Assembly.Location)
        };
        
        return Symbols(source, additionalReferences);
    }
}
```

Then all your test classes inherit from `MyProjectTestBase` and automatically have access to your project's attributes and types.

## Benefits

1. **Single Base Class**: One base class for all Roslyn testing needs
2. **Consistent API**: All methods follow the same pattern
3. **Type Parameters**: Specify analyzer/generator/codefix types at call site, not class level
4. **Flexible**: Easy to mix different test types in the same test class
5. **Extensible**: Override and customize for project-specific needs
