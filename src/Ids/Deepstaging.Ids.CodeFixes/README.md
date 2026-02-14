# Deepstaging.Ids.CodeFixes

Code fix providers for Deepstaging.Ids analyzer diagnostics.

## Overview

Provides automatic code fixes for violations detected by `Deepstaging.Ids.Analyzers`, enabling one-click resolution in
IDEs.

## Code Fixes

| Diagnostic                           | Fix                                                |
|--------------------------------------|----------------------------------------------------|
| `ID0001` — Struct must be partial    | Adds `partial` modifier to the struct declaration  |
| `ID0002` — Struct should be readonly | Adds `readonly` modifier to the struct declaration |

## Dependencies

- `Deepstaging.Ids.Analyzers` — Diagnostic definitions
- `Deepstaging.Roslyn.Workspace` — Code fix infrastructure

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Analyzers](../Deepstaging.Ids.Analyzers/README.md)** — Diagnostics these fixes resolve
- **[Core Attributes](../Deepstaging.Ids/README.md)** — `[StrongId]` attribute definitions
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
