# Deepstaging.HttpClient.Projection

Projection layer for the Deepstaging.HttpClient feature — the single source of truth for HTTP client attribute interpretation.

## Overview

Provides attribute queries and strongly-typed models that bridge Roslyn symbols and the generators/analyzers. All attribute reading and validation logic lives here, ensuring consistent behavior across the toolchain.

## Key Types

### Attribute Queries

| Type | Wraps |
|------|-------|
| `HttpClientAttributeQuery` | `[HttpClient]` — base address, configuration type |
| `HttpVerbAttributeQuery` | `[Get]`, `[Post]`, `[Put]`, `[Patch]`, `[Delete]` — route template, HTTP method |
| `ParameterAttributeQuery` | `[Path]`, `[Query]`, `[Header]`, `[Body]` — parameter binding |

### Models

| Type | Description |
|------|-------------|
| `HttpClientModel` | Top-level client: name, base address, auth, methods |
| `HttpRequestModel` | Single HTTP method: verb, route, parameters, return type |
| `HttpParameterModel` | Single parameter: kind, name, type |
| `HttpVerb` | Enum: `Get`, `Post`, `Put`, `Patch`, `Delete` |
| `ParameterKind` | Enum: `Path`, `Query`, `Header`, `Body` |

## Architecture

```
[HttpClient] attributes → AttributeQueries → Models → Generator / Analyzer
```

## Dependencies

- `Deepstaging.HttpClient` — Attribute definitions
- `Deepstaging.Roslyn` — Projection utilities (`ValidSymbol<T>`, `OptionalAttribute`)

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Core Attributes](../Deepstaging.HttpClient/README.md)** — Attribute definitions
- **[Generators](../Deepstaging.HttpClient.Generators/README.md)** — Consumes projection models
- **[Analyzers](../Deepstaging.HttpClient.Analyzers/README.md)** — Consumes projection models
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit
  - [Projections](https://deepstaging.github.io/roslyn/api/projections/) — Projection pattern

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
