# Deepstaging.HttpClient.Generators

Incremental source generator that produces HTTP client implementations from `[HttpClient]` attributes.

## Overview

This Roslyn source generator triggers on `[HttpClient]` attributes and generates complete `HttpClient`-based implementations, including request construction, parameter binding, serialization, and authentication.

## Generated Output

For a declaration like:

```csharp
[HttpClient(BaseAddress = "https://api.example.com")]
[BearerAuth]
public partial class TodoClient
{
    [Get("/todos/{id}")]
    public partial Task<Todo> GetByIdAsync([Path] int id);

    [Post("/todos")]
    public partial Task<Todo> CreateAsync([Body] CreateTodoRequest request);
}
```

The generator produces:

- **Client partial** — Constructor accepting `HttpClient`, configuration, and auth providers
- **Request methods** — Full implementations with path substitution, query string building, header injection
- **Interface** — `ITodoClient` for dependency injection

## Key Classes

### HttpClientGenerator

```csharp
[Generator]
public class HttpClientGenerator : IIncrementalGenerator
```

Entry point that registers syntax providers for `[HttpClient]` and transforms symbols into models via `Deepstaging.HttpClient.Projection`.

### Writers

| File | Responsibility |
|------|---------------|
| `ClientWriter.cs` | Client class with constructor and configuration |
| `RequestWriter.cs` | Individual HTTP request method implementations |
| `InterfaceWriter.cs` | Extracted interface for DI registration |

## Dependencies

- `Deepstaging.HttpClient.Projection` — Symbol analysis and model building
- `Deepstaging.Roslyn` — Code emission utilities (Emit API)

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Core Attributes](../Deepstaging.HttpClient/README.md)** — `[HttpClient]`, verb and parameter attributes
- **[Projection Models](../Deepstaging.HttpClient.Projection/README.md)** — Semantic analysis layer
- **[Analyzers](../Deepstaging.HttpClient.Analyzers/README.md)** — Compile-time validation
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit
  - [Emit](https://deepstaging.github.io/roslyn/api/emit/) — Code generation API

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
