# Deepstaging.Ids.Projection

Projection layer for the Deepstaging.Ids feature — the single source of truth for `[StrongId]` attribute interpretation.

## Overview

Provides attribute queries and strongly-typed models that bridge Roslyn symbols and the generators/analyzers. All attribute reading and validation logic lives here, ensuring consistent behavior across the toolchain.

## Key Types

### StrongIdAttributeQuery

Wraps `[StrongId]` attribute access with typed property accessors:

```csharp
var query = symbol.StrongIdAttribute();
var backingType = query.BackingType;   // BackingType enum
var converters = query.Converters;     // IdConverters flags
```

### StrongIdModel

Strongly-typed model extracted from the attribute query, used by generators and analyzers:

```csharp
public record StrongIdModel(
    string Name,
    BackingType BackingType,
    IdConverters Converters
);
```

## Architecture

```
[StrongId] attribute → StrongIdAttributeQuery → StrongIdModel → Generator / Analyzer
```

## Dependencies

- `Deepstaging.Ids` — Attribute definitions
- `Deepstaging.Roslyn` — Projection utilities (`ValidSymbol<T>`, `OptionalAttribute`)

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Core Attributes](../Deepstaging.Ids/README.md)** — `[StrongId]`, `BackingType`, `IdConverters`
- **[Generators](../Deepstaging.Ids.Generators/README.md)** — Consumes projection models
- **[Analyzers](../Deepstaging.Ids.Analyzers/README.md)** — Consumes projection models
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit
  - [Projections](https://deepstaging.github.io/roslyn/api/projections/) — Projection pattern

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
