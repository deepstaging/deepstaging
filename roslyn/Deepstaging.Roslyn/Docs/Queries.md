# Queries

Fluent builders for finding types, methods, properties, fields, constructors, events, and parameters without writing loops.

> **See also:** [Projections](Projections.md) | [Emit](Emit.md) | [Extensions](Extensions.md) | [Roslyn Toolkit README](../README.md)

## Overview

Query builders let you compose chainable filters on Roslyn symbols. Each query builder:

- Is immutable (each method returns a new instance)
- Uses lazy evaluation (filters are applied when you call `GetAll()`, `First()`, etc.)
- Returns `ValidSymbol<T>` wrappers with guaranteed non-null access

## TypeQuery

Find types in a compilation or namespace.

```csharp
// Start from a compilation
var types = TypeQuery.From(compilation)
    .ThatArePublic()
    .ThatAreClasses()
    .WithAttribute("MyAttribute")
    .GetAll();

// Start from a namespace
var types = TypeQuery.From(namespaceSymbol)
    .ThatAreInterfaces()
    .InNamespaceStartingWith("MyApp.Domain")
    .GetAll();
```

### Factory Methods

| Method | Description |
|--------|-------------|
| `From(Compilation)` | Query types from the global namespace |
| `From(INamespaceSymbol)` | Query types from a specific namespace |

### Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public types only |
| `ThatAreInternal()` | Internal types only |
| `ThatArePrivate()` | Private types (nested only) |
| `ThatAreProtected()` | Protected types (nested only) |

### Type Kind Filters

| Method | Description |
|--------|-------------|
| `ThatAreClasses()` | Class types |
| `ThatAreInterfaces()` | Interface types |
| `ThatAreStructs()` | Struct types |
| `ThatAreEnums()` | Enum types |
| `ThatAreDelegates()` | Delegate types |
| `ThatAreRecords()` | Record types (class or struct) |

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static types |
| `ThatAreAbstract()` | Abstract types |
| `ThatAreSealed()` | Sealed types |
| `ThatAreGeneric()` | Generic types |
| `ThatArePartial()` | Partial types |
| `ThatAreRefStructs()` | Ref struct types |
| `ThatAreReadOnlyStructs()` | Readonly struct types |

### Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNameStartingWith(string)` | Name starts with prefix |
| `WithNameContaining(string)` | Name contains substring |
| `WithNameEndingWith(string)` | Name ends with suffix |

### Inheritance Filters

| Method | Description |
|--------|-------------|
| `InheritingFrom(INamedTypeSymbol)` | Types inheriting from base type |
| `ImplementingInterface(INamedTypeSymbol)` | Types implementing interface |
| `ImplementingInterface(string)` | Types implementing interface by name |

### Attribute & Namespace Filters

| Method | Description |
|--------|-------------|
| `WithAttribute(string)` | Types with attribute (with or without "Attribute" suffix) |
| `InNamespace(string)` | Exact namespace match |
| `InNamespaceStartingWith(string)` | Namespace starts with prefix |

### Materialization

| Method | Returns | Description |
|--------|---------|-------------|
| `GetAll()` | `ImmutableArray<ValidSymbol<INamedTypeSymbol>>` | All matching types as projections |
| `GetAllSymbols()` | `ImmutableArray<INamedTypeSymbol>` | Raw symbols without wrapper |
| `Select<T>(Func)` | `ImmutableArray<T>` | Project each type to a model |
| `FirstOrDefault()` | `OptionalSymbol<INamedTypeSymbol>` | First match or empty |
| `First()` | `ValidSymbol<INamedTypeSymbol>` | First match or throws |
| `Any()` | `bool` | True if any match |
| `Count()` | `int` | Count of matches |

---

## MethodQuery

Find methods on a type.

```csharp
var asyncMethods = MethodQuery.From(typeSymbol)
    .ThatAreAsync()
    .ThatArePublic()
    .ReturningTask()
    .GetAll();

var handlers = typeSymbol.QueryMethods()
    .WithNameEndingWith("Handler")
    .WithParameterCount(1)
    .GetAll();
```

### Factory Methods

| Method | Description |
|--------|-------------|
| `From(ITypeSymbol)` | Query methods on a type |

You can also use the extension method:
```csharp
typeSymbol.QueryMethods()
```

### Accessibility Filters

| Method | Description |
|--------|-------------|
| `ThatArePublic()` | Public methods |
| `ThatArePrivate()` | Private methods |
| `ThatAreProtected()` | Protected methods |
| `ThatAreInternal()` | Internal methods |
| `ThatAreProtectedOrInternal()` | Protected internal methods |

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static methods |
| `ThatAreInstance()` | Instance methods |
| `ThatAreAsync()` | Async methods |
| `ThatAreGeneric()` | Generic methods |
| `ThatAreVirtual()` | Virtual methods |
| `ThatAreAbstract()` | Abstract methods |
| `ThatAreOverrides()` | Override methods |
| `ThatAreSealed()` | Sealed methods |

### Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNameStartingWith(string)` | Name starts with prefix |
| `WithNameEndingWith(string)` | Name ends with suffix |
| `WithNameContaining(string)` | Name contains substring |

### Parameter Filters

| Method | Description |
|--------|-------------|
| `WithParameterCount(int)` | Exact parameter count |
| `WithNoParameters()` | No parameters |
| `WithParameters()` | At least one parameter |
| `WithFirstParameterOfType(string)` | First parameter matches type name |
| `WithParameters(Func<ImmutableArray<IParameterSymbol>, bool>)` | Custom parameter predicate |

### Return Type Filters

| Method | Description |
|--------|-------------|
| `WithReturnType(string)` | Return type name match |
| `WithReturnType(Func<ITypeSymbol, bool>)` | Custom return type predicate |
| `ReturningVoid()` | Methods returning void |
| `ReturningTask()` | Methods returning Task |
| `ReturningValueTask()` | Methods returning ValueTask |
| `ReturningGenericTask()` | Methods returning Task<T> or ValueTask<T> |

### Attribute Filters

| Method | Description |
|--------|-------------|
| `WithAttribute<TAttribute>()` | Methods with attribute type |
| `WithAttribute(string)` | Methods with attribute by name |
| `WithoutAttribute<TAttribute>()` | Methods without attribute type |

### Materialization

Same as TypeQuery: `GetAll()`, `GetAllSymbols()`, `Select<T>()`, `FirstOrDefault()`, `First()`, `Any()`, `Count()`

---

## PropertyQuery

Find properties on a type.

```csharp
var requiredProps = PropertyQuery.From(typeSymbol)
    .ThatAreRequired()
    .ThatArePublic()
    .GetAll();

var readOnlyProps = typeSymbol.QueryProperties()
    .ThatAreReadOnly()
    .WithoutAttribute<ObsoleteAttribute>()
    .GetAll();
```

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static properties |
| `ThatAreInstance()` | Instance properties |
| `ThatAreVirtual()` | Virtual properties |
| `ThatAreAbstract()` | Abstract properties |
| `ThatAreOverride()` | Override properties |
| `ThatAreSealed()` | Sealed properties |
| `ThatAreReadOnly()` | Read-only (no setter) |
| `ThatAreWriteOnly()` | Write-only (no getter) |
| `ThatAreReadWrite()` | Has both getter and setter |
| `WithInitOnlySetter()` | Init-only setter |
| `ThatAreRequired()` | Required properties |

### Type Filters

| Method | Description |
|--------|-------------|
| `OfType(ITypeSymbol)` | Properties of exact type |
| `OfTypeName(string)` | Properties with type name |
| `OfType(Func<ITypeSymbol, bool>)` | Custom type predicate |

---

## FieldQuery

Find fields on a type.

```csharp
var constants = FieldQuery.From(typeSymbol)
    .ThatAreConst()
    .ThatArePublic()
    .GetAll();

var injectableFields = typeSymbol.QueryFields()
    .WithAttribute("Inject")
    .ThatArePrivate()
    .GetAll();
```

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreStatic()` | Static fields |
| `ThatAreInstance()` | Instance fields |
| `ThatAreReadOnly()` | Readonly fields |
| `ThatAreConst()` | Const fields |
| `ThatAreVolatile()` | Volatile fields |

### Type Filters

| Method | Description |
|--------|-------------|
| `WithType<T>()` | Fields of type T |
| `WithType(string)` | Fields with type name |
| `ThatAreGenericType()` | Fields with generic types |
| `ThatAreNullable()` | Fields with nullable annotation |

### Name Filters

| Method | Description |
|--------|-------------|
| `WithName(string)` | Exact name match |
| `WithNamePrefix(string)` | Name starts with prefix |
| `WithNameSuffix(string)` | Name ends with suffix |
| `WithNameMatching(Func<string, bool>)` | Custom name predicate |

---

## ConstructorQuery

Find constructors on a type.

```csharp
var publicCtors = ConstructorQuery.From(typeSymbol)
    .ThatArePublic()
    .ThatAreInstance()
    .GetAll();

var primaryCtor = typeSymbol.QueryConstructors()
    .WithNoParameters()
    .FirstOrDefault();
```

### Parameter Filters

| Method | Description |
|--------|-------------|
| `WithParameterCount(int)` | Exact parameter count |
| `WithNoParameters()` | Parameterless constructor |
| `WithAtLeastParameters(int)` | Minimum parameter count |
| `WithFirstParameterOfType(ITypeSymbol)` | First parameter matches type |
| `WithParameter(Func<IParameterSymbol, bool>)` | Any parameter matches predicate |
| `WhereAllParameters(Func<IParameterSymbol, bool>)` | All parameters match predicate |

---

## EventQuery

Find events on a type.

```csharp
var publicEvents = EventQuery.From(typeSymbol)
    .ThatArePublic()
    .WithType("EventHandler")
    .GetAll();
```

### Type Filters

| Method | Description |
|--------|-------------|
| `WithType<T>()` | Events of type T |
| `WithType(string)` | Events with type name |

---

## ParameterQuery

Find parameters on a method.

```csharp
var optionalParams = ParameterQuery.From(methodSymbol)
    .ThatAreOptional()
    .GetAll();

var refParams = ParameterQuery.From(methodSymbol)
    .ThatAreRef()
    .GetAll();
```

### Modifier Filters

| Method | Description |
|--------|-------------|
| `ThatAreRef()` | Ref parameters |
| `ThatAreOut()` | Out parameters |
| `ThatAreIn()` | In parameters |
| `ThatAreParams()` | Params array parameters |
| `ThatAreOptional()` | Optional parameters (with defaults) |
| `ThatAreRequired()` | Required parameters |
| `ThatAreThis()` | Extension method 'this' parameter |
| `ThatAreDiscards()` | Discard parameters (named _) |

### Position Filters

| Method | Description |
|--------|-------------|
| `AtPosition(int)` | Parameter at specific index |
| `ThatAreFirst()` | First parameter |
| `ThatAreLast()` | Last parameter |

---

## Custom Filters

All query builders support `Where()` for custom predicates:

```csharp
// Custom predicate escape hatch
var special = TypeQuery.From(compilation)
    .Where(t => t.GetMembers().Length > 10)
    .Where(t => t.ContainingNamespace?.Name == "Domain")
    .GetAll();
```
