# Deepstaging.Ids.Generators

Incremental source generator that produces strongly-typed ID implementations from `[StrongId]` attributes.

## Overview

This Roslyn source generator triggers on `[StrongId]` attributes and generates complete struct implementations including equality, conversion operators, parsing, factory methods, and optional serialization converters.

## Generated Output

For a declaration like:

```csharp
[StrongId(BackingType = BackingType.Guid, Converters = IdConverters.JsonConverter)]
public readonly partial struct OrderId;
```

The generator produces:

- **Core implementation** — `Value` property, constructor, `New()` factory, `Empty`
- **Equality** — `IEquatable<T>`, `==`, `!=`, `GetHashCode()`
- **Conversion** — `ToString()`, `Parse()`, `TryParse()`, implicit/explicit operators
- **Converters** — Requested serialization converters (JSON, EF Core, Dapper, etc.)

## Key Classes

### StrongIdGenerator

```csharp
[Generator]
public class StrongIdGenerator : IIncrementalGenerator
```

Entry point that registers syntax providers for `[StrongId]` and transforms symbols into models via `Deepstaging.Ids.Projection`.

### StrongIdWriter

Partial class with specialized writers for each concern:

| File | Responsibility |
|------|---------------|
| `StrongIdWriter.Core.cs` | Value property, constructor, equality, conversion |
| `StrongIdWriter.Factory.cs` | `New()` and `Empty` factory methods |
| `StrongIdWriter.Converters.cs` | Converter orchestration |
| `StrongIdWriter.JsonConverter.cs` | System.Text.Json converter |
| `StrongIdWriter.EfCoreValueConverter.cs` | EF Core value converter |
| `StrongIdWriter.TypeConverter.cs` | System.ComponentModel type converter |
| `StrongIdWriter.DapperTypeHandler.cs` | Dapper type handler |
| `StrongIdWriter.NewtonsoftJsonConverter.cs` | Newtonsoft.Json converter |

## Dependencies

- `Deepstaging.Ids.Projection` — Symbol analysis and model building
- `Deepstaging.Roslyn` — Code emission utilities (Emit API)

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Core Attributes](../Deepstaging.Ids/README.md)** — `[StrongId]`, `BackingType`, `IdConverters`
- **[Projection Models](../Deepstaging.Ids.Projection/README.md)** — Semantic analysis layer
- **[Analyzers](../Deepstaging.Ids.Analyzers/README.md)** — Compile-time validation
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit
  - [Emit](https://deepstaging.github.io/roslyn/api/emit/) — Code generation API

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
