# Emit API

Fluent builders for generating C# code using Roslyn's SyntaxFactory.

## Quick Start

```csharp
var result = TypeBuilder
    .Class("Customer")
    .InNamespace("MyApp.Domain")
    .AddProperty("Id", "Guid", prop => prop
        .WithAccessibility(Accessibility.Public)
        .WithAutoPropertyAccessors())
    .AddProperty("Name", "string", prop => prop
        .WithAccessibility(Accessibility.Public)
        .WithAutoPropertyAccessors())
    .Emit();

if (result.IsValid(out var validEmit))
{
    string code = validEmit.Code;  // Valid, compilable C#
}
```

## Components

### Builders
- **TypeBuilder** - Classes, interfaces, structs, records
- **PropertyBuilder** - Auto-properties, expression-bodied, block-bodied
- **MethodBuilder** - Instance, static, async methods
- **ConstructorBuilder** - With parameter and this/base chaining
- **FieldBuilder** - Backing fields with modifiers
- **ParameterBuilder** - Method/constructor parameters
- **BodyBuilder** - String-based statement composition

### Projections
- **ValidEmit** - Guaranteed successful emission (non-null code & syntax)
- **OptionalEmit** - May contain diagnostics or errors

### Configuration
- **EmitOptions** - Formatting (indentation, line endings) and validation levels

## Two Builder Patterns

### Lambda Configuration (Concise)
```csharp
.AddMethod("Process", method => method
    .WithReturnType("bool")
    .AddParameter("value", "string")
    .WithBody(b => b.AddReturn("true")))
```

### Separate Builders (Composable)
```csharp
var processMethod = MethodBuilder
    .For("Process")
    .WithReturnType("bool")
    .AddParameter("value", "string")
    .WithBody(b => b.AddReturn("true"));

typeBuilder.AddMethod(processMethod)
```

## Philosophy

The Emit API is the **write counterpart** to the read API:

| Reading | Writing |
|---------|---------|
| TypeQuery | TypeBuilder |
| ValidSymbol | ValidEmit |
| String filters | String types |
| Fluent, immutable | Fluent, immutable |

## Complete Documentation

See **[Emit.md](../Docs/Emit.md)** for comprehensive documentation with examples for every builder and feature.

## Features

✅ Classes, interfaces, structs, records  
✅ Properties (auto, expression-bodied, block-bodied)  
✅ Methods (instance, static, async, virtual, override)  
✅ Constructors (with this/base chaining)  
✅ Fields (readonly, const, static)  
✅ Parameters (with modifiers: ref, out, in, params)  
✅ String-based body building (simple, maintainable)  
✅ Syntax validation (default, opt-out)  
✅ Generates valid, compilable C# code  

## Status

**Phase 1: MVP Complete** ✅  
- String-based type references
- All core builders implemented
- Tested and validated

**Phase 2: Future**  
- Symbol-based type references (TypeReference struct)
- Compilation context support
- Semantic validation

**Phase 3: Future**  
- Expression builder API (if needed)
- Control flow helpers

**Phase 4: Future**  
- Auto-import inference
- EditorConfig integration
- Full C# feature coverage
