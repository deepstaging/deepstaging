# Deepstaging.Projection

Semantic analysis helpers that query Roslyn symbols and build intermediate models from Deepstaging attributes.

## Overview

This library provides extension methods for inspecting `[EffectsModule]` and `[Runtime]` attributes on Roslyn symbols, extracting method signatures, DbSets, and determining async/sync lifting strategies.

## Key Components

### EffectsModule Extensions

Inspects `[EffectsModule]` attributes to extract:

- Target type methods and DbSet properties
- Async/sync method classification
- Include/exclude filters
- Lifting strategy determination

```csharp
var module = symbol.GetEffectsModuleInfo();
var methods = module.GetMethods();
var dbSets = module.GetDbSets();
```

### Runtime Extensions

Builds `RuntimeModel` from `[Runtime]` attributes:

```csharp
var runtime = symbol.GetRuntimeModel();
var capabilities = runtime.AggregateCapabilities();
```

## Usage

These utilities are consumed by `Deepstaging.Generators` to produce source code from the semantic model. They bridge the gap between Roslyn symbol analysis and code generation templates.

## Models

| Model | Description |
|-------|-------------|
| `EffectsModuleModel` | Represents a parsed `[EffectsModule]` declaration |
| `RuntimeModel` | Represents a parsed `[Runtime]` with aggregated modules |
| `MethodModel` | Method signature and lifting strategy |

## Dependencies

- `Deepstaging.Roslyn` - Symbol querying and projections

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Source Generators](../Deepstaging.Generators/README.md)** — Consumer of projection models
- **[Core Attributes](../Deepstaging/README.md)** — Attribute definitions being projected
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit
  - [Queries](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Queries.md) — Symbol query API
  - [Projections](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Projections.md) — Optional/Valid wrappers

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
