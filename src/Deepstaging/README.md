# Deepstaging

The complete Deepstaging toolkit for C#/.NET development.

## What's Included

This umbrella package includes all Deepstaging features:

- **Deepstaging.Effects** — Composable effects system with source generators
- **Deepstaging.Config** — GenerateWith and AutoNotify generators
- **Deepstaging.Ids** — Strongly-typed ID generators
- **Deepstaging.HttpClient** — HTTP client generators

## Installation

```bash
dotnet add package Deepstaging
```

## Individual Packages

If you only need specific features, you can install them individually:

```bash
dotnet add package Deepstaging.Effects
dotnet add package Deepstaging.Config
dotnet add package Deepstaging.Ids
dotnet add package Deepstaging.HttpClient
```

## Testing

`Deepstaging.Effects.Testing` is **not** included in this meta-package and must be installed separately:

```bash
dotnet add package Deepstaging.Effects.Testing
```

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../LICENSE) for the full legal text.
