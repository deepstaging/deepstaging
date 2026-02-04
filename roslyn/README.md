# Roslyn Toolkit

Fluent APIs for building Roslyn source generators, analyzers, and code fixes.

> **See also:** [Main README](../README.md)

## Packages

| Package | Description |
|---------|-------------|
| **[Deepstaging.Roslyn](Deepstaging.Roslyn/README.md)** | Core toolkit — queries, projections, and emit API |
| **[Deepstaging.Roslyn.Scriban](Deepstaging.Roslyn.Scriban/README.md)** | Scriban template infrastructure for generators |
| **[Deepstaging.Roslyn.Workspace](Deepstaging.Roslyn.Workspace/README.md)** | Code fix provider infrastructure |
| **[Deepstaging.Roslyn.Testing](Deepstaging.Roslyn.Testing/README.md)** | Test utilities for all Roslyn components |

## Quick Start

```bash
# Core toolkit
dotnet add package Deepstaging.Roslyn

# Optional: Scriban templates
dotnet add package Deepstaging.Roslyn.Scriban

# Optional: Code fix infrastructure
dotnet add package Deepstaging.Roslyn.Workspace

# Testing (test projects only)
dotnet add package Deepstaging.Roslyn.Testing
```

## What's in Each Package?

### Deepstaging.Roslyn

The core toolkit with three main APIs:

**Queries** — Find symbols without writing loops:
```csharp
TypeQuery.From(compilation)
    .ThatArePublic()
    .ThatAreClasses()
    .WithAttribute("MyAttribute")
    .GetAll();
```

**Projections** — Safe wrappers for nullable Roslyn data:
```csharp
var maxRetries = symbol
    .GetAttribute("RetryAttribute")
    .NamedArg<int>("MaxRetries")
    .OrDefault(3);
```

**Emit** — Generate C# code with fluent builders:
```csharp
TypeBuilder.Class("Customer")
    .InNamespace("MyApp")
    .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
    .Emit();
```

### Deepstaging.Roslyn.Scriban

Template-based code generation for source generators:

```csharp
// In your generator
context.AddFromTemplate(
    Named("MyTemplate.scriban-cs"),
    hintName: $"{model.Name}.g.cs",
    context: model);
```

### Deepstaging.Roslyn.Workspace

Simplified code fix providers:

```csharp
[CodeFix("MY001")]
public class AddPartialFix : SyntaxCodeFix<TypeDeclarationSyntax>
{
    protected override CodeAction? CreateFix(Document document, ValidSyntax<TypeDeclarationSyntax> syntax)
        => CodeAction.Create("Add partial", c => AddPartial(document, syntax.Node, c));
}
```

### Deepstaging.Roslyn.Testing

Test all Roslyn components from a single base class:

```csharp
public class MyTests : RoslynTestBase
{
    [Test]
    public async Task TestGenerator()
    {
        await GenerateWith<MyGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public partial class")
            .VerifySnapshot();
    }
}
```

## Documentation

### Core Toolkit
- [Queries](Deepstaging.Roslyn/Docs/Queries.md) — Find types, methods, properties, and more
- [Projections](Deepstaging.Roslyn/Docs/Projections.md) — Safe nullable symbol wrappers
- [Emit](Deepstaging.Roslyn/Docs/Emit.md) — Generate C# code with fluent builders
- [Extensions](Deepstaging.Roslyn/Docs/Extensions.md) — Convenience methods for Roslyn types

### Testing
- [RoslynTestBase](Deepstaging.Roslyn.Testing/Docs/RoslynTestBase.md) — Unified testing API
- [SymbolTestContext](Deepstaging.Roslyn.Testing/Docs/SymbolTestContext.md) — Symbol querying in tests
- [AnalyzerTestContext](Deepstaging.Roslyn.Testing/Docs/AnalyzerTestContext.md) — Analyzer assertions
- [GeneratorTestContext](Deepstaging.Roslyn.Testing/Docs/GeneratorTestContext.md) — Generator assertions
- [CodeFixTestContext](Deepstaging.Roslyn.Testing/Docs/CodeFixTestContext.md) — Code fix assertions
- [ReferenceConfiguration](Deepstaging.Roslyn.Testing/Docs/ReferenceConfiguration.md) — Test compilation references

## Design Philosophy

This is utility code, not a framework. It should feel like Roslyn's missing standard library.

- **Reading and writing are symmetric** — TypeQuery finds types, TypeBuilder creates types
- **Fluent and immutable** — Chain methods, get new instances
- **You still get Roslyn** — Call `.GetAll()` and get real `ISymbol[]`

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../LICENSE) for the full legal text.
