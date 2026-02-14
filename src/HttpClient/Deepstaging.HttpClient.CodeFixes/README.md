# Deepstaging.HttpClient.CodeFixes

Code fix providers for Deepstaging.HttpClient analyzer diagnostics.

## Overview

Provides automatic code fixes for violations detected by `Deepstaging.HttpClient.Analyzers`, enabling one-click resolution in IDEs.

## Code Fixes

| Diagnostic | Fix |
|------------|-----|
| `HTTP001` — HttpClient class must be partial | Adds `partial` modifier to the class declaration |
| `HTTP002` — HTTP method must be partial | Adds `partial` modifier to the method declaration |

## Dependencies

- `Deepstaging.HttpClient.Analyzers` — Diagnostic definitions
- `Deepstaging.Roslyn.Workspace` — Code fix infrastructure

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Analyzers](../Deepstaging.HttpClient.Analyzers/README.md)** — Diagnostics these fixes resolve
- **[Core Attributes](../Deepstaging.HttpClient/README.md)** — `[HttpClient]` attribute definitions
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
