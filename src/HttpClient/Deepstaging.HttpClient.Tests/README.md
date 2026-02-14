# Deepstaging.HttpClient.Tests

Test suite for the Deepstaging.HttpClient source generator and analyzers.

## Overview

Uses TUnit and the `Deepstaging.Roslyn.Testing` infrastructure to verify generator output (via snapshot testing) and
analyzer/code fix behavior.

## Test Categories

| Test Class                 | Covers                                                                                       |
|----------------------------|----------------------------------------------------------------------------------------------|
| `HttpClientGeneratorTests` | Generator output for client declarations with various verb, parameter, and auth combinations |

## Running

```bash
dotnet test deepstaging/src/HttpClient/Deepstaging.HttpClient.Tests
```

## Dependencies

- `Deepstaging.Roslyn.Testing` — `RoslynTestBase`, `GenerateWith`, `AnalyzeWith`
- `TUnit` — Test framework

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Core Attributes](../Deepstaging.HttpClient/README.md)** — What's being tested
- **[Generators](../Deepstaging.HttpClient.Generators/README.md)** — Generator under test
- **[Analyzers](../Deepstaging.HttpClient.Analyzers/README.md)** — Analyzers under test

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
