# Deepstaging.Ids.Tests

Test suite for the Deepstaging.Ids source generator and analyzers.

## Overview

Uses TUnit and the `Deepstaging.Roslyn.Testing` infrastructure to verify generator output (via snapshot testing) and
analyzer/code fix behavior.

## Test Categories

| Test Class                              | Covers                                                            |
|-----------------------------------------|-------------------------------------------------------------------|
| `StrongIdGeneratorTests`                | Generator output for all backing types and converter combinations |
| `StrongIdMustBePartialAnalyzerTests`    | `ID0001` diagnostic reporting                                     |
| `StrongIdMustBePartialCodeFixTests`     | `ID0001` code fix application                                     |
| `StrongIdShouldBeReadonlyAnalyzerTests` | `ID0002` diagnostic reporting                                     |
| `StrongIdShouldBeReadonlyCodeFixTests`  | `ID0002` code fix application                                     |

## Running

```bash
dotnet test deepstaging/src/Ids/Deepstaging.Ids.Tests
```

## Dependencies

- `Deepstaging.Roslyn.Testing` — `RoslynTestBase`, `GenerateWith`, `AnalyzeWith`, `AnalyzeAndFixWith`
- `TUnit` — Test framework

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Core Attributes](../Deepstaging.Ids/README.md)** — What's being tested
- **[Generators](../Deepstaging.Ids.Generators/README.md)** — Generator under test
- **[Analyzers](../Deepstaging.Ids.Analyzers/README.md)** — Analyzers under test

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
