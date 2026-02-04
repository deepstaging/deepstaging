# Copilot Instructions

This repository contains **Deepstaging**, a Roslyn-based code generation toolkit for C#/.NET.

## Build & Test Commands

```bash
# Build entire solution
dotnet build Deepstaging.slnx

# Run all tests
dotnet test Deepstaging.slnx

# Run a specific test by name
dotnet test --filter "FullyQualifiedName~MyTestClassName.MyTestMethod"

# Run tests in a specific project
dotnet test roslyn/Deepstaging.Roslyn.Tests
```

## Architecture Overview

### Solution Structure

- **`src/`** - Main application code
  - `Deepstaging` - Core package with marker attributes (entry point for consumers)
  - `Deepstaging.Generators` - Incremental source generators
  - `Deepstaging.Analyzers` - Roslyn diagnostic analyzers
  - `Deepstaging.Runtime` - Runtime support library
  - `Deepstaging.Tests` - Integration tests

- **`roslyn/`** - Roslyn utilities (the core toolkit)
  - `Deepstaging.Roslyn` - Query builders, projections, emit API, and generator extensions
  - `Deepstaging.Roslyn.Scriban` - Scriban template infrastructure for generators
  - `Deepstaging.Roslyn.Testing` - Test base classes for Roslyn components
  - `Deepstaging.Roslyn.Tests` - Tests for the Roslyn toolkit

### Roslyn Library (Deepstaging.Roslyn)

Three main APIs for working with Roslyn:

1. **Queries** - Fluent builders for finding symbols:
   ```csharp
   TypeQuery.From(compilation)
       .ThatArePublic()
       .ThatAreClasses()
       .WithAttribute("MyAttribute")
       .GetAll();
   ```

2. **Projections** - Optional/validated wrappers for nullable symbols:
   ```csharp
   symbol.GetAttribute("MyAttribute")
       .NamedArg<int>("MaxRetries")
       .OrDefault(3);
   ```

3. **Emit** - Fluent builders for generating C# code:
   ```csharp
   TypeBuilder.Class("Customer")
       .InNamespace("MyApp")
       .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
       .Emit();
   ```

### Source Generator Pattern

Generators use Scriban templates with this structure:

```
Deepstaging.Generators/
├── DeepstagingGenerator.cs
├── Writers/           # Template invocation logic
└── Templates/          # .scriban-cs templates (embedded resources)
```

Template usage:
```csharp
private static readonly Func<string, TemplateName> Named = 
    TemplateName.ForGenerator<MyGenerator>();

context.AddFromTemplate(
    Named("MyTemplate.scriban-cs"),
    hintName,
    model);
```

## Testing Conventions

### Test Framework

Uses **TUnit** (not xUnit/NUnit) with async assertions:
```csharp
await Assert.That(result).HasCount(2);
await Assert.That(name).IsEqualTo("Expected");
```

### Roslyn Testing Base Classes

All Roslyn tests inherit from `RoslynTestBase`:

```csharp
public class MyTests : RoslynTestBase
{
    [Test]
    public async Task TestSymbols()
    {
        var type = Symbols("public class MyClass { }").GetType("MyClass");
        // ...
    }

    [Test]
    public async Task TestAnalyzer()
    {
        await Analyze<MyAnalyzer>(source)
            .ShouldReportDiagnostic("MYID001");
    }

    [Test]
    public async Task TestGenerator()
    {
        await Generate<MyGenerator>(source)
            .ShouldGenerate()
            .VerifySnapshot();
    }
}
```

### Reference Configuration

Tests needing custom assembly references configure them via `ModuleInitializer`:
```csharp
[ModuleInitializer]
public static void Init() =>
    ReferenceConfiguration.AddReferencesFromTypes(typeof(MyAttribute));
```

### Snapshot Testing

Uses **Verify** for generator output verification. Snapshot files are stored alongside test files with `.verified.txt` extension.

## Code Conventions

### Build Configuration

- **TreatWarningsAsErrors** is enabled globally - all warnings must be fixed
- **Nullable reference types** are enabled
- **Central package management** via `Directory.Packages.props`
- Build outputs go to `artifacts/` directory

### Query/Emit Symmetry

The library maintains symmetry between reading and writing:
- `TypeQuery` finds types → `TypeBuilder` creates types
- `MethodQuery` finds methods → `MethodBuilder` creates methods
- `ValidSymbol<T>` wraps symbols → `ValidEmit` wraps generated code

### Projection Pattern

Use `Optional*` and `Valid*` wrappers instead of null checks:
```csharp
// Early exit pattern
if (optional.IsNotValid(out var valid))
    return;
// valid is now guaranteed non-null
```

### Template Naming

Scriban templates use `.scriban-cs` extension and are embedded resources:
```xml
<EmbeddedResource Include="Templates\*.scriban-cs" />
```
