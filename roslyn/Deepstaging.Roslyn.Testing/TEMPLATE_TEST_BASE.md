# TemplateTestBase

A test base class for testing Scriban template rendering with symbol queries and snapshot verification.

## Overview

`TemplateTestBase<TGenerator>` provides a fluent API for testing template rendering in source generators. It combines:
- **Symbol queries** from `SymbolTestBase` to extract symbols from compiled code
- **Template rendering** from the generator's template system
- **Snapshot testing** with Verify for output validation

## Basic Usage

```csharp
public class MyTemplateTests : TemplateTestBase<MyGenerator>
{
    [Test]
    public async Task RenderMyTemplate()
    {
        var source = """
            namespace MyNamespace;
            
            public class MyClass
            {
                public string Name { get; set; }
            }
            """;
        
        await Template(source)
            .Render("MyTemplate.scriban-cs", symbols =>
            {
                // Query symbols to build template context
                var type = symbols.RequireNamedType("MyClass");
                return new MyTemplateModel(type);
            })
            .ShouldRender()
            .WithContent("public class MyClass")
            .VerifySnapshot();
    }
}
```

## Key Features

### 1. Template Context Creation

The `Template(source)` method creates a compilation from your source code and provides access to both symbols and template rendering:

```csharp
var template = Template(source);

// Access symbols directly
var myType = template.Symbols.RequireNamedType("MyClass");

// Or use in template rendering
await template.Render("Template.scriban-cs", symbols => {...})
```

### 2. Template Rendering

Two ways to provide context to templates:

**With symbol query:**
```csharp
await Template(source)
    .Render("Template.scriban-cs", symbols =>
    {
        var type = symbols.RequireNamedType("MyClass");
        return new TemplateModel(type);
    })
    .ShouldRender()
    .VerifySnapshot();
```

**With direct context:**
```csharp
var model = new TemplateModel(...);

await Template(source)
    .Render("Template.scriban-cs", model)
    .ShouldRender()
    .VerifySnapshot();
```

### 3. Template Naming

Use the `Named()` helper to get template names without knowing internals:

```csharp
var templateName = Named("MyTemplate.scriban-cs");
// Expands to: "MyNamespace.Generator.Templates.MyTemplate.scriban-cs"
```

### 4. Assertions

**Success assertions:**
```csharp
await Template(source)
    .Render("Template.scriban-cs", context)
    .ShouldRender()  // Assert render succeeds
    .WithContent("expected text")  // Assert output contains text
    .VerifySnapshot();  // Snapshot test the output
```

**Failure assertions:**
```csharp
await Template(source)
    .Render("BadTemplate.scriban-cs", context)
    .ShouldFail();  // Assert render fails
```

### 5. Symbol Queries

Full access to symbol querying from `SymbolTestBase`:

```csharp
var template = Template(source);

// Query types
var myClass = template.Symbols.RequireNamedType("MyClass");

// Query with projections
var result = template.Symbols.Query(
    s => s.RequireNamedType("MyClass"),
    (symbol, compilation) => MyProjection(symbol, compilation));
```

## Template File Naming

Templates must follow the generator's namespace convention:
- Located in: `{GeneratorNamespace}.Templates`
- File name: As specified in your generator (e.g., "RuntimeCore.scriban-cs")

The `TemplateName.ForGenerator<TGenerator>()` automatically constructs the full namespace path.

## Example: Testing with Effects Generator

```csharp
public class RuntimeCoreTemplateTests : TemplateTestBase<EffectsGenerator>
{
    [Test]
    public async Task RenderRuntimeCore_WithNoDependencies()
    {
        var source = """
            using Deepstaging.Effects;
            
            namespace TestNamespace;
            
            [Runtime]
            public sealed partial class MyRuntime
            {
            }
            """;

        await Template(source)
            .Render("RuntimeCore.scriban-cs", symbols =>
            {
                var runtimeType = symbols.RequireNamedType("MyRuntime");
                var systemInfo = runtimeType.QueryEffectsSystemInfo(symbols.Compilation);
                return new RuntimeCoreModel(systemInfo);
            })
            .ShouldRender()
            .WithContent("public sealed partial class MyRuntime")
            .VerifySnapshot();
    }
}
```

## Snapshot Files

Snapshots are stored next to your test file:
- Test file: `MyTests.cs`
- Snapshot: `MyTests.RenderMyTemplate.verified.txt`

Use Verify's features for snapshot management (auto-approve, diff tools, etc.).

## Relation to Other Test Bases

- **`SymbolTestBase`**: For testing symbol queries and projections
- **`GeneratorTestBase<T>`**: For testing end-to-end generator output
- **`TemplateTestBase<T>`**: For testing individual template rendering (this class)

Choose based on what you're testing:
- Query logic → `SymbolTestBase`
- Template rendering → `TemplateTestBase<T>`
- Full generation → `GeneratorTestBase<T>`
