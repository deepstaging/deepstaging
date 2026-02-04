# Projections

Optional and validated wrappers that make null-checking less painful. Inspired by functional Option types.

> **See also:** [Queries](Queries.md) | [Emit](Emit.md) | [Extensions](Extensions.md) | [Roslyn Toolkit README](../README.md)

## Overview

Roslyn symbols are often nullable, requiring constant null checks. Projections wrap these nullable values and provide:

- **OptionalSymbol/OptionalAttribute/OptionalValue/OptionalArgument** - May or may not contain a value
- **ValidSymbol/ValidAttribute** - Guaranteed non-null, created via validation

## The Pattern

```csharp
// Without projections - null checks everywhere
var attr = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "MyAttribute");
if (attr == null) return;
var value = attr.ConstructorArguments.FirstOrDefault().Value;
if (value is not string s) return;
// finally use s

// With projections - fluent null-safe operations
var value = symbol
    .GetAttribute("MyAttribute")
    .ConstructorArg<string>(0)
    .OrDefault("fallback");
```

---

## OptionalSymbol<T>

Wraps a Roslyn symbol that may or may not be present.

### Creating

```csharp
// From a value
OptionalSymbol<INamedTypeSymbol>.WithValue(typeSymbol)

// Empty
OptionalSymbol<INamedTypeSymbol>.Empty()

// From nullable
OptionalSymbol<INamedTypeSymbol>.FromNullable(maybeNull)
```

### Checking Presence

```csharp
if (optional.HasValue) { /* has symbol */ }
if (optional.IsEmpty) { /* no symbol */ }
```

### Extracting Values

```csharp
// Get or throw
var symbol = optional.OrThrow("Symbol required");

// Get or null
var maybeNull = optional.OrNull();

// Validate to non-nullable wrapper
if (optional.IsValid(out var valid))
{
    // valid is ValidSymbol<T> with guaranteed non-null
    Console.WriteLine(valid.Name);
}

// Early exit pattern
if (optional.IsNotValid(out var valid))
    return;
// valid is now ValidSymbol<T>
```

### Transforming

```csharp
// Map to a different type
OptionalValue<string> name = optional.Map(s => s.FullyQualifiedName);

// Filter
OptionalSymbol<T> filtered = optional.Where(s => s.IsPublic());

// Cast to derived type
OptionalSymbol<IMethodSymbol> method = symbol.OfType<IMethodSymbol>();
```

### Properties

Common symbol properties are available directly on the optional:

```csharp
optional.Name               // string? - symbol name
optional.Namespace          // string? - containing namespace
optional.FullyQualifiedName // string? - full name without global::
optional.GloballyQualifiedName // string? - full name with global::
optional.DisplayName        // string? - namespace.name format
optional.PropertyName       // string? - suggested property name
optional.ParameterName      // string? - suggested parameter name
optional.Location           // Location - primary location

// Accessibility
optional.Accessibility      // Accessibility? enum
optional.IsPublic           // bool
optional.IsInternal         // bool
optional.IsPrivate          // bool
optional.IsProtected        // bool
optional.AccessibilityString // string? - "public", "private", etc.

// Modifiers
optional.IsStatic           // bool
optional.IsAbstract         // bool
optional.IsSealed           // bool
optional.IsVirtual          // bool
optional.IsOverride         // bool
optional.IsReadOnly         // bool
optional.IsPartial          // bool

// Type classification
optional.IsGenericType      // bool
optional.IsValueType        // bool
optional.IsReferenceType    // bool
optional.IsInterface        // bool
optional.IsClass            // bool
optional.IsStruct           // bool
optional.IsRecord           // bool
optional.IsEnum             // bool
optional.IsDelegate         // bool
optional.IsNullable         // bool
optional.Kind               // string? - "class", "struct", "interface", etc.

// Method-specific
optional.IsAsync            // bool
optional.IsExtensionMethod  // bool

// Task support
optional.IsTask             // bool
optional.InnerTaskType      // inner type of Task<T>/ValueTask<T>
```

### Generic Type Support

```csharp
// Get type arguments
ImmutableArray<OptionalSymbol<INamedTypeSymbol>> args = optional.GetTypeArguments();

// Get specific argument
OptionalValue<INamedTypeSymbol> firstArg = optional.GetTypeArgument(0);
OptionalSymbol<ITypeSymbol> argSymbol = optional.GetTypeArgumentSymbol(0);

// Single type argument (for generics with exactly one type parameter)
OptionalSymbol<ITypeSymbol> single = optional.SingleTypeArgument;

// Type parameters
IEnumerable<OptionalSymbol<ITypeParameterSymbol>> typeParams = optional.GetTypeParameters();
```

### Attributes

```csharp
// Get all attributes
IEnumerable<OptionalAttribute> attrs = optional.GetAttributes();

// Get by name
IEnumerable<ValidAttribute> attrs = optional.GetAttributes("MyAttribute");

// Get by type
IEnumerable<ValidAttribute> attrs = optional.GetAttributes<ObsoleteAttribute>();

// Get first by name
OptionalAttribute attr = optional.GetAttribute("MyAttribute");

// Get first by type
OptionalAttribute attr = optional.GetAttribute<ObsoleteAttribute>();
```

### Utility Methods

```csharp
// Execute action if present
optional.Do(s => Console.WriteLine(s.Name));

// Pattern matching
optional.Match(
    whenPresent: s => HandleSymbol(s),
    whenEmpty: () => HandleEmpty());
```

---

## ValidSymbol<T>

A validated symbol where the underlying value is guaranteed non-null.

### Creating

```csharp
// From non-null symbol (throws if null)
ValidSymbol<INamedTypeSymbol>.From(typeSymbol)

// Try to create (returns null if input is null)
ValidSymbol<INamedTypeSymbol>? maybe = ValidSymbol<INamedTypeSymbol>.TryFrom(typeSymbol);

// From OptionalSymbol validation
if (optional.IsValid(out var valid)) { /* use valid */ }
```

### Properties

Same properties as OptionalSymbol, but return non-nullable types:

```csharp
valid.Name                  // string (not nullable)
valid.FullyQualifiedName    // string (not nullable)
valid.Accessibility         // Accessibility (not nullable)
valid.Value                 // TSymbol - the underlying symbol
```

### Transforming

```csharp
// Map to any type
TResult result = valid.Map(s => s.Name);

// Map to another ValidSymbol
ValidSymbol<IMethodSymbol> method = valid.MapTo(s => (IMethodSymbol)s);

// Filter (returns null if predicate fails)
ValidSymbol<T>? filtered = valid.Where(s => s.IsPublic);

// Cast (returns null if cast fails)
ValidSymbol<IMethodSymbol>? method = valid.OfType<IMethodSymbol>();
```

---

## OptionalAttribute

Wraps an AttributeData that may or may not be present.

### Creating

```csharp
OptionalAttribute.WithValue(attributeData)
OptionalAttribute.Empty()
OptionalAttribute.FromNullable(maybeNull)
```

### Getting Arguments

```csharp
// Constructor argument by index
OptionalArgument<string> arg = attr.ConstructorArg<string>(0);
OptionalArgument<int> count = attr.ConstructorArg<int>(1);

// Named argument
OptionalArgument<int> retries = attr.NamedArg<int>("MaxRetries");
OptionalArgument<string> message = attr.NamedArg<string>("Message");
```

### Generic Attribute Type Arguments

```csharp
// Get type arguments from generic attributes like [MyAttribute<TRuntime, TEvent>]
ImmutableArray<OptionalSymbol<INamedTypeSymbol>> typeArgs = attr.GetTypeArguments();
OptionalArgument<INamedTypeSymbol> firstTypeArg = attr.GetTypeArgument(0);
```

### Transforming

```csharp
// Map to a result type
OptionalArgument<MyConfig> config = attr.Map(a => new MyConfig(a));

// Extract multiple arguments at once
OptionalArgument<MyConfig> config = attr.WithArgs(a => new MyConfig
{
    Name = a.ConstructorArg<string>(0).OrDefault("Default"),
    Retries = a.NamedArg<int>("MaxRetries").OrDefault(3)
});
```

### Validation

```csharp
if (attr.IsValid(out var valid))
{
    // valid is ValidAttribute with guaranteed non-null
}

if (attr.IsNotValid(out var valid))
    return; // early exit

ValidAttribute validated = attr.ValidateOrThrow("Attribute required");
```

---

## ValidAttribute

A validated attribute with guaranteed non-null AttributeData.

```csharp
// Same argument extraction methods as OptionalAttribute
OptionalArgument<string> arg = validAttr.ConstructorArg<string>(0);
OptionalArgument<int> retries = validAttr.NamedArg<int>("MaxRetries");

// Access attribute class directly
INamedTypeSymbol attrClass = validAttr.AttributeClass;

// Get type arguments from generic attributes
ImmutableArray<ValidSymbol<INamedTypeSymbol>> typeArgs = validAttr.GetTypeArguments();
```

---

## OptionalArgument<T>

Wraps an attribute argument value that may or may not be present.

### Extracting Values

```csharp
// Get or default
string value = arg.OrDefault("fallback");
int count = arg.OrDefault(() => ComputeDefault());

// Get or throw
string value = arg.OrThrow("Argument required");
string value = arg.OrThrow(() => new CustomException());

// Get or null
string? maybeNull = arg.OrNull();
```

### Transforming

```csharp
// Map
OptionalArgument<int> length = arg.Map(s => s.Length);

// Convert int to enum (Roslyn stores enums as ints)
OptionalArgument<MyEnum> enumValue = arg.ToEnum<MyEnum>();
```

### Pattern Matching

```csharp
string result = arg.Match(
    whenPresent: v => $"Value: {v}",
    whenEmpty: () => "No value");

// Try pattern
if (arg.TryGetValue(out var value))
{
    Console.WriteLine(value);
}

// Early exit pattern
if (arg.IsMissing(out var value))
    return;
```

---

## OptionalValue<T>

Generic optional wrapper for any value type (not specific to Roslyn symbols).

Same API as OptionalArgument but for general use:

```csharp
OptionalValue<string>.WithValue("hello")
OptionalValue<string>.Empty()

value.Map(s => s.Length)
value.OrDefault("fallback")
value.OrThrow()
```

---

## Real-World Examples

### Extract Attribute Configuration

```csharp
var config = symbol
    .GetAttribute("RetryAttribute")
    .WithArgs(a => new RetryConfig
    {
        MaxRetries = a.NamedArg<int>("MaxRetries").OrDefault(3),
        DelayMs = a.NamedArg<int>("DelayMs").OrDefault(1000),
        ExponentialBackoff = a.NamedArg<bool>("Exponential").OrDefault(false)
    })
    .OrDefault(RetryConfig.Default);
```

### Safe Type Navigation

```csharp
var elementType = typeSymbol
    .AsNamedType()
    .Where(t => t.IsGenericType && t.Name == "List")
    .Map(t => t.Value.SingleTypeArgument)
    .OrDefault(OptionalSymbol<ITypeSymbol>.Empty());
```

### Validate Before Processing

```csharp
public void Process(OptionalSymbol<IMethodSymbol> method)
{
    if (method.IsNotValid(out var valid))
    {
        ReportError("Method symbol required");
        return;
    }
    
    // valid is ValidSymbol<IMethodSymbol> - no null checks needed
    var name = valid.Name;
    var returnType = valid.Map(m => m.ReturnType);
}
```

### Chain Optional Operations

```csharp
var serviceName = symbol
    .GetAttribute("ServiceAttribute")
    .ConstructorArg<INamedTypeSymbol>(0)
    .Map(t => t.Name)
    .OrDefault(() => symbol.Name + "Service");
```
