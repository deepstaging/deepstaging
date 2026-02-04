# Deepstaging

A Roslyn-based code generation toolkit for C#/.NET with two main purposes:

1. **Effects System** — Generate composable, testable effect wrappers for your services with OpenTelemetry instrumentation
2. **Roslyn Toolkit** — Fluent APIs for building your own source generators, analyzers, and code fixes

## Installation

```bash
# Effects system (source generators + runtime)
dotnet add package Deepstaging

# Roslyn toolkit (for building generators)
dotnet add package Deepstaging.Roslyn
dotnet add package Deepstaging.Roslyn.Scriban  # Optional: Scriban template support
```

---

## Part 1: Effects System

Turn your service interfaces into composable, instrumented effects with zero boilerplate.

### Quick Start

**1. Define your services**

```csharp
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

public interface ISlackService
{
    Task PostMessageAsync(string channel, string message);
}
```

**2. Create an effects module**

```csharp
using Deepstaging;

[EffectsModule(typeof(IEmailService), Name = "Email")]
[EffectsModule(typeof(ISlackService), Name = "Slack")]
public partial class AppEffects;
```

**3. Define your runtime**

```csharp
[Runtime]
[Uses(typeof(AppEffects))]
public partial class AppRuntime;
```

**4. Compose and run effects**

```csharp
using static AppEffects;

// Compose effects with LINQ-style syntax
var workflow = 
    from _ in Email.Send("user@example.com", "Hello", "Welcome!")
    from _ in Slack.PostMessage("#general", "New user signed up!")
    select unit;

// Run with your runtime
await workflow.Run(runtime);
```

### Features

| Feature | Description |
|---------|-------------|
| **Zero Reflection** | Compile-time source generation for maximum performance |
| **OpenTelemetry** | Built-in tracing with `Activity` spans (zero overhead when disabled) |
| **LanguageExt** | First-class `Eff<RT, A>` integration for functional composition |
| **Analyzers** | Catch configuration errors at compile time |
| **Code Fixes** | Quick fixes for common mistakes |

### Configuration Options

```csharp
[EffectsModule(
    typeof(IEmailService),
    Name = "Email",              // Module name (default: derived from type)
    Instrumented = true,         // Enable OpenTelemetry tracing (default: true)
    IncludeOnly = ["SendAsync"], // Only wrap these methods
    Exclude = ["Ping"]           // Exclude these methods
)]
public partial class EmailModule;
```

---

## Part 2: Roslyn Toolkit

Build your own source generators with fluent APIs for querying symbols, projecting data, and emitting code.

### Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Your Generator                           │
├─────────────────────────────────────────────────────────────────┤
│  Queries          │  Projections       │  Emit                  │
│  ───────          │  ───────────       │  ────                  │
│  TypeQuery        │  OptionalSymbol<T> │  TypeBuilder           │
│  MethodQuery      │  ValidSymbol<T>    │  MethodBuilder         │
│  PropertyQuery    │  OptionalAttribute │  PropertyBuilder       │
│  FieldQuery       │  ValidAttribute    │  FieldBuilder          │
│  ParameterQuery   │  XmlDocumentation  │  ConstructorBuilder    │
│  ConstructorQuery │                    │  AttributeBuilder      │
│  EventQuery       │                    │  BodyBuilder           │
└─────────────────────────────────────────────────────────────────┘
```

### Queries — Find Symbols

Fluent builders for discovering types, methods, properties, and other symbols:

```csharp
// Find all public classes with a specific attribute
var types = TypeQuery.From(compilation)
    .ThatArePublic()
    .ThatAreClasses()
    .ThatArePartial()
    .WithAttribute("GenerateAttribute")
    .GetAll();

// Find async methods returning Task<T>
var methods = MethodQuery.From(typeSymbol)
    .ThatArePublic()
    .ThatAreAsync()
    .WithReturnTypeMatching(t => t.Name == "Task")
    .GetAll();

// Find properties with a getter
var properties = PropertyQuery.From(typeSymbol)
    .ThatArePublic()
    .ThatHaveGetter()
    .GetAll();
```

**Available Query Types:**

| Query | Purpose |
|-------|---------|
| `TypeQuery` | Find classes, interfaces, structs, records, enums |
| `MethodQuery` | Find methods with filters for async, static, return type |
| `PropertyQuery` | Find properties with getter/setter filters |
| `FieldQuery` | Find fields with const/readonly filters |
| `ParameterQuery` | Query method/constructor parameters |
| `ConstructorQuery` | Find constructors by accessibility and parameters |
| `EventQuery` | Find events on types |

### Projections — Safe Symbol Access

Wrap nullable Roslyn symbols with null-safe accessors and validation:

```csharp
// OptionalSymbol<T> — May or may not have a value
var optional = symbol.GetAttribute("MyAttribute");

if (optional.HasValue)
{
    var name = optional.Name;  // Safe access
}

// Early-exit pattern with IsNotValid
if (optional.IsNotValid(out var valid))
    return;  // Exit if not valid

// valid is now ValidSymbol<T> with guaranteed non-null access
var typeName = valid.Name;

// Chain attribute access
var maxRetries = symbol
    .GetAttribute("RetryAttribute")
    .NamedArg("MaxRetries")
    .OrDefault(3);

// Access constructor arguments
var targetType = symbol
    .GetAttribute("GenerateAttribute")
    .ConstructorArg(0)
    .AsType()
    .OrThrow("Target type is required");
```

**Projection Types:**

| Type | Purpose |
|------|---------|
| `OptionalSymbol<T>` | Nullable symbol wrapper with fluent accessors |
| `ValidSymbol<T>` | Guaranteed non-null symbol after validation |
| `OptionalAttribute` | Nullable attribute data with argument access |
| `ValidAttribute` | Guaranteed non-null attribute after validation |
| `XmlDocumentation` | Parse and access XML doc comments |

### Emit — Generate Code

Fluent builders for constructing C# code:

```csharp
// Build a class with properties and methods
var code = TypeBuilder.Class("CustomerDto")
    .InNamespace("MyApp.Models")
    .AsPartial()
    .WithUsing("System")
    .WithUsing("System.Text.Json.Serialization")
    .AddProperty("Id", "int", p => p
        .WithGetter()
        .WithSetter())
    .AddProperty("Name", "string", p => p
        .WithGetter()
        .WithInitOnlySetter()
        .WithAttribute("JsonPropertyName", a => a.WithArgument("\"name\"")))
    .AddMethod("Validate", "bool", m => m
        .AsPublic()
        .WithBody(b => b
            .AddStatement("return !string.IsNullOrEmpty(Name)")))
    .Emit();

// Build an interface
var interfaceCode = TypeBuilder.Interface("IRepository")
    .WithTypeParameter("T")
    .AddMethod("GetByIdAsync", "Task<T?>", m => m
        .WithParameter("id", "int"))
    .AddMethod("SaveAsync", "Task", m => m
        .WithParameter("entity", "T"))
    .Emit();
```

**Builder Types:**

| Builder | Purpose |
|---------|---------|
| `TypeBuilder` | Classes, interfaces, structs, records |
| `MethodBuilder` | Methods with parameters, body, attributes |
| `PropertyBuilder` | Properties with getters, setters, init-only |
| `FieldBuilder` | Fields, constants, readonly fields |
| `ConstructorBuilder` | Instance and static constructors |
| `ParameterBuilder` | Method/constructor parameters |
| `AttributeBuilder` | Attributes with arguments |
| `BodyBuilder` | Method bodies from C# strings |
| `XmlDocumentationBuilder` | XML doc comments |

### Scriban Templates

For complex code generation, use Scriban templates:

```csharp
// In your generator
private static readonly Func<string, TemplateName> Named = 
    TemplateName.ForGenerator<MyGenerator>();

context.AddFromTemplate(
    Named("MyTemplate.scriban-cs"),
    $"{typeName}.g.cs",
    new { TypeName = typeName, Properties = properties });
```

```scriban
// Templates/MyTemplate.scriban-cs
namespace {{ namespace }};

public partial class {{ type_name }}
{
{{~ for prop in properties ~}}
    public {{ prop.type }} {{ prop.name }} { get; set; }
{{~ end ~}}
}
```

### Analyzers

Build analyzers with the `SymbolAnalyzer` base class:

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(Diagnostics.MissingPartialKeyword)]
public class MissingPartialAnalyzer : TypeAnalyzer
{
    protected override void Analyze(
        SymbolAnalysisContext context,
        INamedTypeSymbol type)
    {
        if (!type.IsPartial() && type.HasAttribute("GenerateAttribute"))
        {
            context.ReportDiagnostic(
                Diagnostics.MissingPartialKeyword,
                type.Locations.First());
        }
    }
}
```

---

## Project Structure

```
src/
├── Deepstaging              # Core attributes (EffectsModule, Runtime, Uses)
├── Deepstaging.Generators   # Source generators for effects
├── Deepstaging.Analyzers    # Diagnostic analyzers
├── Deepstaging.CodeFixes    # Code fix providers
├── Deepstaging.Runtime      # Runtime support (OpenTelemetry, EF Core)
├── Deepstaging.Projection   # Query models for Deepstaging attributes

roslyn/
├── Deepstaging.Roslyn          # Core toolkit (Queries, Projections, Emit)
├── Deepstaging.Roslyn.Scriban  # Scriban template integration
├── Deepstaging.Roslyn.Testing  # Test utilities for generators/analyzers
```

## Build & Test

```bash
# Build
dotnet build Deepstaging.slnx

# Test
dotnet test Deepstaging.slnx

# Run specific test
dotnet test --filter "FullyQualifiedName~MyTestClass.MyTestMethod"
```

## Documentation

- **[Roslyn Toolkit](roslyn/Deepstaging.Roslyn/README.md)** - Fluent APIs for queries, projections, and code emission
  - [Queries](roslyn/Deepstaging.Roslyn/Docs/Queries.md) - Find types, methods, properties, and more
  - [Projections](roslyn/Deepstaging.Roslyn/Docs/Projections.md) - Safe nullable symbol wrappers
  - [Emit](roslyn/Deepstaging.Roslyn/Docs/Emit.md) - Generate C# code with fluent builders
  - [Extensions](roslyn/Deepstaging.Roslyn/Docs/Extensions.md) - Convenience methods for Roslyn types
- **[Testing](roslyn/Deepstaging.Roslyn.Testing/README.md)** - Test infrastructure for analyzers and generators
  - [RoslynTestBase](roslyn/Deepstaging.Roslyn.Testing/Docs/RoslynTestBase.md) - Unified testing API
  - [Reference Configuration](roslyn/Deepstaging.Roslyn.Testing/Docs/ReferenceConfiguration.md) - Configure test compilation references
  - [Template Testing](roslyn/Deepstaging.Roslyn.Testing/TEMPLATE_TEST_BASE.md) - Test Scriban templates
- **[Build Configuration](build/README.md)** - MSBuild configuration files

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](LICENSE) for the full legal text.
