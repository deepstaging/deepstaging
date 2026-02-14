# Deepstaging.Ids.Analyzers

Roslyn analyzers that enforce strongly-typed ID best practices at compile time.

## Overview

Provides diagnostic analyzers that validate `[StrongId]` attribute usage, ensuring correct struct declarations before source generation runs.

## Diagnostics

| ID | Severity | Description |
|----|----------|-------------|
| `ID0001` | Error | StrongId struct must be declared as `partial` |
| `ID0002` | Warning | StrongId struct should be declared as `readonly` |

## Example Error

```csharp
// ID0001: StrongId struct 'UserId' must be declared as partial
[StrongId]
public struct UserId;  // ❌ Missing 'partial'
```

Fix:
```csharp
[StrongId]
public readonly partial struct UserId;  // ✅
```

## Dependencies

- `Deepstaging.Ids.Projection` — Symbol analysis and model building
- `Deepstaging.Roslyn` — Roslyn analysis utilities

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Core Attributes](../Deepstaging.Ids/README.md)** — `[StrongId]`, `BackingType`, `IdConverters`
- **[Code Fixes](../Deepstaging.Ids.CodeFixes/README.md)** — Automatic fixes for these diagnostics
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
