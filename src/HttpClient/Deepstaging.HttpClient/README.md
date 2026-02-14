# Deepstaging.HttpClient

Source-generated HTTP clients for C#/.NET — declarative, type-safe, zero boilerplate.

## Overview

This package provides attributes that mark partial classes as HTTP clients. The source generator produces full
`HttpClient`-based implementations from interface-style declarations, including path/query/header parameter binding and
authentication.

## Usage

```csharp
using Deepstaging.HttpClient;

[HttpClient(BaseAddress = "https://api.example.com")]
public partial class TodoClient
{
    [Get("/todos")]
    public partial Task<List<Todo>> GetAllAsync();

    [Get("/todos/{id}")]
    public partial Task<Todo> GetByIdAsync([Path] int id);

    [Post("/todos")]
    public partial Task<Todo> CreateAsync([Body] CreateTodoRequest request);

    [Delete("/todos/{id}")]
    public partial Task DeleteAsync([Path] int id);
}
```

## Attributes

### [HttpClient]

Marks a partial class as an HTTP client.

| Property      | Type      | Description               |
|---------------|-----------|---------------------------|
| `BaseAddress` | `string?` | Base URL for all requests |

A generic variant `[HttpClient<TConfiguration>]` supports typed configuration injection.

### HTTP Verb Attributes

| Attribute  | HTTP Method |
|------------|-------------|
| `[Get]`    | GET         |
| `[Post]`   | POST        |
| `[Put]`    | PUT         |
| `[Patch]`  | PATCH       |
| `[Delete]` | DELETE      |

All verb attributes accept a route template (e.g., `"/todos/{id}"`).

### Parameter Attributes

| Attribute  | Description                                                    |
|------------|----------------------------------------------------------------|
| `[Path]`   | Substituted into the route template (e.g., `{id}`)             |
| `[Query]`  | Appended as a query string parameter; optional `Name` override |
| `[Header]` | Sent as an HTTP header; requires header name                   |
| `[Body]`   | Serialized as the request body                                 |

### Authentication Attributes

| Attribute      | Description                                 |
|----------------|---------------------------------------------|
| `[BearerAuth]` | Bearer token via `ITokenProvider`           |
| `[ApiKeyAuth]` | API key with configurable header name       |
| `[BasicAuth]`  | Basic authentication with username/password |

## Analyzers

| ID        | Severity | Description                                           |
|-----------|----------|-------------------------------------------------------|
| `HTTP001` | Error    | HttpClient class must be partial                      |
| `HTTP002` | Error    | HTTP method must be partial                           |
| `HTTP003` | Error    | HTTP method must not return Task (use a typed return) |
| `HTTP004` | Error    | HTTP path must not be empty                           |

## Installation

```bash
dotnet add package Deepstaging.HttpClient
```

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Generators](../Deepstaging.HttpClient.Generators/README.md)** — How code is generated
- **[Analyzers](../Deepstaging.HttpClient.Analyzers/README.md)** — Compile-time validation
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
