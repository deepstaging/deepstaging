# Extensions

Convenience methods for common Roslyn operations. These extend standard Roslyn types with fluent, null-safe APIs.

## Overview

Extension methods are organized by the type they extend:

- **ISymbol** - Accessibility checks, attribute access
- **ITypeSymbol** - Query builders, type checks
- **INamedTypeSymbol** - Interface checks, type conversions
- **AttributeData** - Argument extraction
- **INamespaceSymbol** - Namespace traversal
- **Compilation** - Symbol resolution

## ISymbol Extensions

Available on all Roslyn symbols.

### Symbol Equality

```csharp
// Semantic equality check
bool same = symbol.IsSymbol(otherSymbol);
bool different = symbol.DoesNotEqual(otherSymbol);
```

### Attributes

```csharp
// Get all attributes as ValidAttribute projections
IEnumerable<ValidAttribute> attrs = symbol.GetAttributes();

// Get by name (supports both "Name" and "NameAttribute" forms)
IEnumerable<ValidAttribute> attrs = symbol.GetAttributesByName("Obsolete");

// Get by type
IEnumerable<ValidAttribute> attrs = symbol.GetAttributesByType<ObsoleteAttribute>();
```

### Accessibility Checks

```csharp
symbol.IsPublic()
symbol.IsPrivate()
symbol.IsProtected()
symbol.IsInternal()
symbol.IsProtectedInternal()
symbol.IsPrivateProtected()
```

### Modifier Checks

```csharp
symbol.IsVirtual()
symbol.IsOverride()
symbol.IsSealed()
symbol.IsAbstract()
symbol.IsStatic()
symbol.IsExtern()
symbol.IsObsolete()       // Has [Obsolete] attribute
symbol.IsImplicitlyDeclared()
```

### Source Location

```csharp
symbol.IsFromSource()     // Defined in source code
symbol.IsFromMetadata()   // From referenced assembly
```

---

## ITypeSymbol Extensions

Start query builders and check type characteristics.

### Query Builders

```csharp
// Start fluent queries
MethodQuery methods = typeSymbol.QueryMethods();
PropertyQuery properties = typeSymbol.QueryProperties();
FieldQuery fields = typeSymbol.QueryFields();
ConstructorQuery constructors = typeSymbol.QueryConstructors();
EventQuery events = typeSymbol.QueryEvents();

// Get attributes
ImmutableArray<AttributeData> attrs = typeSymbol.QueryAttributes();
```

### Task Type Checks

```csharp
typeSymbol.IsTaskType()           // Task or ValueTask (with or without T)
typeSymbol.IsValueTaskType()      // ValueTask or ValueTask<T>
typeSymbol.IsGenericTaskType()    // Task<T>
typeSymbol.IsGenericValueTaskType() // ValueTask<T>
typeSymbol.IsNonGenericTaskType() // Task (no type arg)
typeSymbol.IsNonGenericValueTaskType() // ValueTask (no type arg)
```

### Collection Type Checks

```csharp
typeSymbol.IsEnumerableType()     // IEnumerable<T>
typeSymbol.IsCollectionType()     // ICollection<T>
typeSymbol.IsListType()           // IList<T>
typeSymbol.IsDictionaryType()     // IDictionary<TKey, TValue>
typeSymbol.IsQueryableType()      // IQueryable<T>
typeSymbol.IsObservableType()     // IObservable<T>
```

### Special Type Checks

```csharp
typeSymbol.IsNullableValueType()  // Nullable<T>
typeSymbol.IsFuncType()           // Func<...>
typeSymbol.IsActionType()         // Action or Action<...>
typeSymbol.IsLazyType()           // Lazy<T>
typeSymbol.IsTupleType()          // ValueTuple
typeSymbol.IsArrayType()          // T[]
typeSymbol.IsPointerType()        // T*
```

### Type Kind Checks

```csharp
typeSymbol.IsDelegateType()       // Delegate
typeSymbol.IsEnumType()           // Enum
typeSymbol.IsInterfaceType()      // Interface
typeSymbol.IsRecordType()         // Record class or record struct
typeSymbol.IsStructType()         // Struct (excluding enums)
typeSymbol.IsClassType()          // Class
typeSymbol.IsAbstractType()       // Abstract
typeSymbol.IsSealedType()         // Sealed
typeSymbol.IsStaticType()         // Static
```

### Inheritance

```csharp
// Check if implements or inherits
bool result = typeSymbol.ImplementsOrInheritsFrom(baseType);

// Check by name
bool isException = typeSymbol.IsOrInheritsFrom("Exception", "System");

// Excludes the type itself
bool inherits = typeSymbol.InheritsFrom("Controller", "Microsoft.AspNetCore.Mvc");

// Get base type by name
ITypeSymbol? baseType = typeSymbol.GetBaseTypeByName("DbContext");
```

### Type Arguments

```csharp
// Get single type argument from generic types like List<T>
ITypeSymbol? elementType = typeSymbol.GetSingleTypeArgument();
```

---

## INamedTypeSymbol Extensions

Additional extensions for named types.

### Conversion to Projections

```csharp
// Wrap in OptionalSymbol
OptionalSymbol<INamedTypeSymbol> optional = symbol.AsNamedType();

// Wrap in ValidSymbol (throws if null)
ValidSymbol<INamedTypeSymbol> valid = symbol.AsValidNamedType();
```

### Interface Checks

```csharp
// Check if implements interface by name
bool isDisposable = namedType.HasInterface("IDisposable");
bool isGenericEnumerable = namedType.HasInterface("IEnumerable", arity: 1);
```

---

## AttributeData Extensions

Extract attribute arguments fluently.

### Wrap in Projection

```csharp
// Wrap for fluent querying
OptionalAttribute optional = attributeData.Query();
```

### Get Arguments

```csharp
// Named argument
OptionalArgument<int> retries = attributeData.GetNamedArgument<int>("MaxRetries");
int value = retries.OrDefault(3);

// Constructor argument by index
OptionalArgument<string> name = attributeData.GetConstructorArgument<string>(0);
string value = name.OrThrow("Name required");
```

### Collection Extensions

```csharp
// Filter attributes by name from a collection
IEnumerable<ValidAttribute> attrs = attributeDataList.GetByName("MyAttribute");
```

---

## INamespaceSymbol Extensions

Navigate namespace hierarchies.

```csharp
// Get all types in namespace (including nested)
IEnumerable<INamedTypeSymbol> types = namespaceSymbol.GetAllTypes();

// Get all types recursively
IEnumerable<INamedTypeSymbol> allTypes = namespaceSymbol.GetAllTypesRecursively();

// Check if namespace is nested under another
bool isNested = namespaceSymbol.IsNestedIn("MyApp.Domain");
```

---

## Compilation Extensions

Resolve well-known types and symbols.

```csharp
// Get well-known types
INamedTypeSymbol? taskType = compilation.GetTaskType();
INamedTypeSymbol? cancellationToken = compilation.GetCancellationTokenType();

// Resolve type by name
INamedTypeSymbol? type = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1");
```

---

## Usage Examples

### Find Async Methods Returning Entity Types

```csharp
var asyncMethods = typeSymbol.QueryMethods()
    .ThatArePublic()
    .ThatAreAsync()
    .Where(m => m.ReturnType.IsGenericTaskType())
    .GetAll();
```

### Check If Type Is Repository

```csharp
bool isRepo = typeSymbol.IsInterfaceType() &&
              typeSymbol.Name.EndsWith("Repository") &&
              typeSymbol.HasInterface("IRepository");
```

### Extract Attribute Configuration

```csharp
var config = symbol.GetAttributesByName("Configure")
    .FirstOrDefault()
    .Map(attr => new Config
    {
        Name = attr.ConstructorArg<string>(0).OrDefault("Default"),
        Enabled = attr.NamedArg<bool>("Enabled").OrDefault(true),
        Priority = attr.NamedArg<int>("Priority").OrDefault(0)
    })
    .OrDefault(Config.Default);
```

### Validate Symbol Before Processing

```csharp
var optional = symbol.AsNamedType();
if (optional.IsNotValid(out var valid))
{
    context.ReportDiagnostic(/* ... */);
    return;
}

// valid is ValidSymbol<INamedTypeSymbol> - all properties non-null
var name = valid.FullyQualifiedName;
var methods = valid.Value.QueryMethods().ThatArePublic().GetAll();
```

### Check Type Hierarchy

```csharp
// Is this a controller?
if (typeSymbol.InheritsFrom("ControllerBase", "Microsoft.AspNetCore.Mvc"))
{
    var actions = typeSymbol.QueryMethods()
        .ThatArePublic()
        .ThatAreInstance()
        .WithoutAttribute<NonActionAttribute>()
        .GetAll();
}
```
