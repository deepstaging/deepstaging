# HTTP Client Module

The HTTP Client module generates typed HTTP clients from annotated partial classes.

## Overview

```csharp
using Deepstaging.HttpClient;

[HttpClient<ApiConfig>]
[BearerAuth]
public partial class UsersClient
{
    [Get("/users/{id}")]
    private partial User GetUser(int id);

    [Post("/users")]
    private partial User CreateUser([Body] CreateUserRequest request);

    [Get("/users")]
    private partial List<User> ListUsers([Query] int page, [Header("X-Request-Id")] string requestId);
}
```

The generator produces:

- **Client implementation** — `HttpClient`-backed methods with serialization
- **`IUsersClient` interface** — for DI and testing
- **Request types** — strongly-typed request records

## Attributes

### Class-Level

| Attribute | Description |
|-----------|-------------|
| `[HttpClient]` / `[HttpClient<TConfig>]` | Marks a partial class as an HTTP client. Optional `BaseAddress`. |
| `[BearerAuth]` | Adds bearer token authentication |
| `[ApiKeyAuth]` | API key authentication. `HeaderName` required, `ConfigProperty` optional. |
| `[BasicAuth]` | Basic authentication. `UsernameProperty`, `PasswordProperty`. |

### Method-Level (HTTP Verbs)

| Attribute | Description |
|-----------|-------------|
| `[Get(path)]` | HTTP GET |
| `[Post(path)]` | HTTP POST |
| `[Put(path)]` | HTTP PUT |
| `[Patch(path)]` | HTTP PATCH |
| `[Delete(path)]` | HTTP DELETE |

Path supports `{param}` templates: `"/users/{id}/orders/{orderId}"`

### Parameter-Level

| Attribute | Description |
|-----------|-------------|
| `[Path]` | Path parameter (auto-detected from template) |
| `[Query]` | Query string parameter. Optional `Name` override. |
| `[Header(name)]` | HTTP header |
| `[Body]` | Request body (serialized as JSON) |

## Diagnostics

| ID | Severity | Description |
|----|----------|-------------|
| DSHTTP01 | Error | HttpClient class must be partial |
| DSHTTP02 | Error | HTTP method must be partial |
| DSHTTP03 | Error | HTTP method must not return Task |
| DSHTTP04 | Error | HTTP path must not be empty |
