# Deepstaging.Testing

Testing infrastructure for Roslyn analyzers, generators, and source code transformations.

## What is this?

A comprehensive testing framework that makes it easy to test Roslyn tooling at every layer:

1. **Query testing** - Test symbol extraction and projections (fastest)
2. **Template testing** - Test source code generation with snapshot verification
3. **Analyzer testing** - Test diagnostic rules and messages
4. **CodeFix testing** - Test code transformations
5. **Generator testing** - Integration tests for full generator pipelines

## Why this approach?

Most Roslyn testing jumps straight to end-to-end generator tests, which are slow and provide poor feedback. Deepstaging promotes a **query-first, layered testing approach**:

```
┌──────────────────────────────────────────────────────┐
│ Generator Tests (Integration smoke tests)           │
│    ↓ Full pipeline confidence                        │
├──────────────────────────────────────────────────────┤
│ CodeFix Tests (Transformation logic)                │
│    ↓ Builds on proven analyzers                      │
├──────────────────────────────────────────────────────┤
│ Analyzer Tests (Diagnostic rules)                   │
│    ↓ Uses proven queries                             │
├──────────────────────────────────────────────────────┤
│ Template Tests (Rendering logic)                    │
│    ↓ Uses proven queries                             │
├──────────────────────────────────────────────────────┤
│ Query Tests (FOUNDATION - Start here!)              │
│    Fast, focused, everything builds on these         │
└──────────────────────────────────────────────────────┘
```

**Focus on query tests first** - they're fast (milliseconds), focused, and everything else builds on them.

## Quick Start

### Installation

```bash
dotnet add package Deepstaging.Testing
```

### Configuring References

If your tests need references to your own assemblies, configure them once before tests run:

```csharp
using System.Runtime.CompilerServices;
using Deepstaging.Testing;

internal static class TestInit
{
    [ModuleInitializer]
    public static void Init() =>
        ReferenceConfiguration.AddReferencesFromTypes(typeof(MyFeature));
}
```

See [REFERENCE_CONFIGURATION.md](REFERENCE_CONFIGURATION.md) for details.

### Your First Test

```csharp
using Deepstaging.Testing;

public class MyTests : SymbolTestBase
{
    [Test]
    public async Task ShouldFindProperties()
    {
        var type = Symbols("""
            public class MyClass
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
            """)
            .GetType("MyClass");

        var properties = type.GetProperties();
        await Assert.That(properties).HasCount(2);
    }
}
```

---

## Testing Base Classes

### SymbolTestBase

Use this when testing code that queries or projects Roslyn symbols (e.g., extension methods on `ITypeSymbol`).

#### Features

**Get Type by Name:**
```csharp
var type = Symbols("public class MyClass { }").GetType("MyClass");
```

**Query Properties:**
```csharp
var properties = await Symbols("""
    public class MyClass
    {
        public int Id { get; set; }
        private string Secret { get; set; }
    }
    """)
    .Type("MyClass")
    .Properties()
    .ThatArePublic()
    .Execute();

await Assert.That(properties).HasCount(1);
```

**Query Methods:**
```csharp
var methods = await Symbols(source)
    .Type("MyClass")
    .Methods()
    .ThatArePublic()
    .Execute();
```

**Query All Types (in source code under test):**
```csharp
// This queries only types in your test source code (excludes System.*, etc.)
var classes = await Symbols(source)
    .Types()
    .ThatAreClasses()
    .ThatArePublic()
    .Execute();
```

**Query All Types (including referenced assemblies):**
```csharp
// This queries all types in the entire compilation (use sparingly)
var allTypes = await Symbols(source)
    .AllTypesInCompilation()
    .ThatAreClasses()
    .Execute();
```

**Custom Projections:**
```csharp
var model = Symbols(source).Project("MyClass", type => type.ToTypeModel());
```

#### Example

```csharp
public class TypeExtensionsTests : SymbolTestBase
{
    [Test]
    public async Task GetProperties_ShouldReturnAllProperties()
    {
        var type = Symbols("""
            public class TestClass
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
            """)
            .GetType("TestClass");

        var properties = type.GetProperties();
        await Assert.That(properties).HasCount(2);
    }
}
```

---

### AnalyzerTestBase<TAnalyzer>

Use this when testing diagnostic analyzers.

#### Features

**Assert Diagnostic is Reported:**
```csharp
await Analyze(source)
    .ShouldReportDiagnostic("DEEPRT001");
```

**Assert with Severity:**
```csharp
await Analyze(source)
    .ShouldReportDiagnostic("DEEPRT001")
    .WithSeverity(DiagnosticSeverity.Error);
```

**Assert with Message Pattern:**
```csharp
await Analyze(source)
    .ShouldReportDiagnostic("DEEPRT001")
    .WithMessage("*must be partial*");  // Supports wildcards
```

**Assert Diagnostic NOT Reported:**
```csharp
await Analyze(source)
    .ShouldNotReportDiagnostic("DEEPRT001");
```

#### Example

```csharp
using Deepstaging.Testing;
using Deepstaging.Runtime.Analyzers;

public class RuntimeAnalyzerTests : AnalyzerTestBase<RuntimeAnalyzer>
{
    [Test]
    public async Task DEEPRT001_ShouldReportError_WhenNotPartial()
    {
        await Analyze("""
            using Deepstaging.Runtime;
            
            [Runtime]
            public sealed class Runtime { }
            """)
            .ShouldReportDiagnostic("DEEPRT001")
            .WithSeverity(DiagnosticSeverity.Error);
    }
    
    [Test]
    public async Task DEEPRT001_ShouldNotReportError_WhenPartial()
    {
        await Analyze("""
            [Runtime]
            public sealed partial class Runtime { }
            """)
            .ShouldNotReportDiagnostic("DEEPRT001");
    }
}
```

---

### GeneratorTestBase

Use this when testing source generators.

#### Features

**Assert Generation Occurs:**
```csharp
await Generate<MyGenerator>(source)
    .ShouldGenerate();
```

**Assert No Generation:**
```csharp
await Generate<MyGenerator>(source)
    .ShouldNotGenerate();
```

**Assert File Count:**
```csharp
await Generate<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileCount(2);
```

**Assert File Name:**
```csharp
await Generate<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileNamed("Runtime.Generated.g.cs");
```

**Assert Content:**
```csharp
await Generate<MyGenerator>(source)
    .ShouldGenerate()
    .WithFileContaining("public partial class Runtime");
```

**Snapshot Verification:**
```csharp
await Generate<MyGenerator>(source)
    .ShouldGenerate()
    .VerifySnapshot();
```

#### Example

```csharp
using Deepstaging.Testing;

public class MyGeneratorTests : GeneratorTestBase
{
    [Test]
    public async Task ShouldGenerateCode_WhenAttributePresent()
    {
        await Generate<MyGenerator>("""
            [MyAttribute]
            public partial class Example { }
            """)
            .ShouldGenerate()
            .WithFileCount(1)
            .WithFileNamed("Example.Generated.g.cs")
            .VerifySnapshot();
    }
    
    [Test]
    public async Task ShouldNotGenerate_WhenAttributeMissing()
    {
        await Generate<MyGenerator>("""
            public partial class Example { }
            """)
            .ShouldNotGenerate();
    }
}
```

---

## Core Utilities

### CompilationHelper

Internal utility for creating test compilations with proper references. **Most tests should use base classes instead.**

#### When to Use CompilationHelper Directly

Use `CompilationHelper` when you need fine-grained control over compilations or when writing infrastructure code:

```csharp
// Create compilation
var compilation = CompilationHelper.CreateCompilation(source);

// Get default references
var references = CompilationHelper.GetDefaultReferences();
```

#### Location
```
src/shared/Deepstaging.Common.Testing/CompilationHelper.cs
```

---

### WellKnownSymbols

Provides easy access to commonly used framework type symbols.

#### Available Symbols

```csharp
// Primitive types
WellKnownSymbols.String       // System.String
WellKnownSymbols.Int32        // System.Int32
WellKnownSymbols.Boolean      // System.Boolean
WellKnownSymbols.Object       // System.Object
WellKnownSymbols.Void         // System.Void
WellKnownSymbols.DateTime     // System.DateTime

// Task types
WellKnownSymbols.Task         // Task (non-generic)
WellKnownSymbols.TaskOfT      // Task<T>
WellKnownSymbols.ValueTask    // ValueTask (non-generic)
WellKnownSymbols.ValueTaskOfT // ValueTask<T>

// Collection types
WellKnownSymbols.ListOfT              // List<T>
WellKnownSymbols.DictionaryOfTKeyTValue // Dictionary<TKey, TValue>
WellKnownSymbols.IEnumerableOfT       // IEnumerable<T>

// Attributes
WellKnownSymbols.ObsoleteAttribute    // ObsoleteAttribute
```

#### Example

```csharp
[Test]
public async Task TestWithFrameworkTypes()
{
    var taskSymbol = WellKnownSymbols.Task;
    var stringSymbol = WellKnownSymbols.String;
    
    await Assert.That(taskSymbol.Name).IsEqualTo("Task");
    await Assert.That(stringSymbol.Name).IsEqualTo("String");
}
```

#### Cache Management

```csharp
// Clear cache for test isolation
WellKnownSymbols.ClearCache();
```

---

## API Reference

### SymbolTestContext

| Method | Returns | Description |
|--------|---------|-------------|
| `GetType(string)` | `INamedTypeSymbol` | Get type by simple name |
| `GetNamespace(string)` | `INamespaceSymbol` | Get namespace by qualified name |
| `Type(string)` | `TypeQueryContext` | Start querying a specific type |
| `Types()` | `TypeQuery` | Query types in source code under test (excludes referenced assemblies) |
| `AllTypesInCompilation()` | `TypesQueryContext` | Query all types including referenced assemblies (use sparingly) |
| `Project<T>(string, Func)` | `T` | Custom projection on type |

### AnalyzerTestContext

| Method | Returns | Description |
|--------|---------|-------------|
| `ShouldReportDiagnostic(string)` | `DiagnosticAssertion` | Assert diagnostic is reported |
| `ShouldNotReportDiagnostic(string)` | `Task` | Assert diagnostic is NOT reported |

### DiagnosticAssertion

| Method | Returns | Description |
|--------|---------|-------------|
| `WithSeverity(DiagnosticSeverity)` | `DiagnosticAssertion` | Assert severity level |
| `WithMessage(string)` | `DiagnosticAssertion` | Assert message pattern (wildcards supported) |

### GeneratorTestContext

| Method | Returns | Description |
|--------|---------|-------------|
| `ShouldGenerate()` | `GeneratorAssertions` | Assert generation occurs |
| `ShouldNotGenerate()` | `Task` | Assert no generation |

### GeneratorAssertions

| Method | Returns | Description |
|--------|---------|-------------|
| `WithFileCount(int)` | `GeneratorAssertions` | Assert number of files |
| `WithFileNamed(string)` | `GeneratorAssertions` | Assert file name exists |
| `WithFileContaining(string)` | `GeneratorAssertions` | Assert content present |
| `VerifySnapshot()` | `Task` | Verify using Verify snapshot testing |

---

## Examples

### Testing Extension Methods

```csharp
public class TypeExtensionsTests : SymbolTestBase
{
    [Test]
    public async Task GetPublicProperties_ShouldReturnOnlyPublic()
    {
        var type = Symbols("""
            public class MyClass
            {
                public int PublicProp { get; set; }
                private string PrivateProp { get; set; }
            }
            """)
            .GetType("MyClass");

        var publicProps = type.GetPublicProperties().ToList();
        
        await Assert.That(publicProps).HasCount(1);
        await Assert.That(publicProps[0].Name).IsEqualTo("PublicProp");
    }
}
```

### Testing Analyzer Diagnostics

```csharp
public class MyAnalyzerTests : AnalyzerTestBase<MyAnalyzer>
{
    [Test]
    public async Task ShouldReportWarning_WhenConditionMet()
    {
        await Analyze("""
            [MyAttribute]
            public class BadClass { }
            """)
            .ShouldReportDiagnostic("MYID001")
            .WithSeverity(DiagnosticSeverity.Warning)
            .WithMessage("*not allowed*");
    }

    [Test]
    public async Task ShouldNotReportWarning_WhenConditionNotMet()
    {
        await Analyze("""
            public class GoodClass { }
            """)
            .ShouldNotReportDiagnostic("MYID001");
    }
}
```

### Testing Source Generator

```csharp
public class MyGeneratorTests : GeneratorTestBase
{
    [Test]
    public async Task ShouldGenerateCode_WhenAttributePresent()
    {
        await Generate<MyGenerator>("""
            [GenerateExtensions]
            public partial class MyClass { }
            """)
            .ShouldGenerate()
            .WithFileCount(1)
            .WithFileNamed("MyClass.Extensions.g.cs")
            .WithFileContaining("public static class MyClassExtensions")
            .VerifySnapshot();
    }

    [Test]
    public async Task ShouldNotGenerate_WhenAttributeMissing()
    {
        await Generate<MyGenerator>("""
            public partial class MyClass { }
            """)
            .ShouldNotGenerate();
    }
}
```

### Using WellKnownSymbols

```csharp
public class TaskReturnTypeTests : SymbolTestBase
{
    [Test]
    public async Task ShouldDetectTaskReturnType()
    {
        var type = Symbols("""
            public class MyClass
            {
                public Task DoSomethingAsync() => Task.CompletedTask;
            }
            """)
            .GetType("MyClass");

        var method = type.GetMethods().First(m => m.Name == "DoSomethingAsync");
        var taskSymbol = WellKnownSymbols.Task;
        
        await Assert.That(SymbolEqualityComparer.Default.Equals(
            method.ReturnType, 
            taskSymbol)).IsTrue();
    }
}
```

---

## Tips & Best Practices

1. **Use fluent APIs** - They're more readable and less error-prone
2. **Leverage awaitable contexts** - Most contexts are awaitable for immediate verification
3. **Chain assertions** - Multiple assertions can be chained for concise tests
4. **Keep tests focused** - One assertion pattern per test method
5. **Use snapshot testing** - For generators, snapshot testing catches unexpected changes
6. **Use raw string literals** - Use `"""..."""` for multi-line source code (C# 11+)
7. **Include usings** - Add `using` directives if you reference framework types
8. **Test both positive and negative cases** - Use `ShouldGenerate()` and `ShouldNotGenerate()`
9. **Use `Types()` for your code** - It queries only your test source code, not System.* types
10. **Use `AllTypesInCompilation()` sparingly** - Only when you need to query framework or dependency types

---

## Project Structure

```
src/shared/Deepstaging.Common.Testing/
├── AnalyzerTestBase.cs          # Base class for analyzer tests
├── CompilationHelper.cs         # Compilation creation utility
├── GeneratorTestBase.cs         # Base class for generator tests
├── SymbolTestBase.cs            # Base class for symbol tests
├── WellKnownSymbols.cs          # Framework type symbols
├── Contexts/
│   ├── AnalyzerTestContext.cs   # Fluent analyzer assertions
│   ├── GeneratorTestContext.cs  # Fluent generator assertions
│   └── SymbolTestContext.cs     # Fluent symbol queries
├── Extensions/
│   └── GeneratorDriverRunResultExtensions.cs
└── README.md                    # This file
```

---

## See Also

- [Deepstaging Documentation](../../../docs/)
- [Generator Implementation Guide](../../../.claude/docs/generators/IMPLEMENTING_GENERATORS.md)
- [Roslyn API Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/)

---

## License

MIT License - See repository root for details.
