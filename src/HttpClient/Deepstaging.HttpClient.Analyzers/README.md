# Deepstaging.HttpClient.Analyzers

Roslyn analyzers that enforce HTTP client best practices at compile time.

## Overview

Provides diagnostic analyzers that validate `[HttpClient]` and HTTP verb attribute usage, ensuring correct declarations
before source generation runs.

## Diagnostics

| ID        | Severity | Description                                                            |
|-----------|----------|------------------------------------------------------------------------|
| `HTTP001` | Error    | `[HttpClient]` class must be declared as `partial`                     |
| `HTTP002` | Error    | HTTP method must be declared as `partial`                              |
| `HTTP003` | Error    | HTTP method must not return `Task` (use a typed return like `Task<T>`) |
| `HTTP004` | Error    | HTTP path must not be empty                                            |

## Example Error

```csharp
// HTTP001: HttpClient class 'TodoClient' must be declared as partial
[HttpClient]
public class TodoClient;  // ❌ Missing 'partial'
```

Fix:

```csharp
[HttpClient]
public partial class TodoClient;  // ✅
```

## Dependencies

- `Deepstaging.HttpClient.Projection` — Symbol analysis and model building
- `Deepstaging.Roslyn` — Roslyn analysis utilities

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Core Attributes](../Deepstaging.HttpClient/README.md)** — `[HttpClient]`, verb and parameter attributes
- **[Code Fixes](../Deepstaging.HttpClient.CodeFixes/README.md)** — Automatic fixes for these diagnostics
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
