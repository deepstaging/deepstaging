---
applyTo: src/**/HttpClient/**,src/**/*HttpClient*
---

# HttpClient Module

Namespace: `Deepstaging.HttpClient`

Generates typed HTTP clients with request/response models from annotated partial classes.

## Attributes

**Class-level:**
- `HttpClientAttribute` / `HttpClientAttribute<TConfiguration>` — `BaseAddress` (optional)
- `BearerAuthAttribute` — bearer token authentication
- `ApiKeyAuthAttribute` — `HeaderName` (required), `ConfigProperty`
- `BasicAuthAttribute` — `UsernameProperty`, `PasswordProperty`

**Method-level (HTTP verbs):**
- `GetAttribute`, `PostAttribute`, `PutAttribute`, `PatchAttribute`, `DeleteAttribute` — `Path` (required, supports `{param}` templates)

**Parameter-level:**
- `PathAttribute` — path parameter (auto-detected from template)
- `QueryAttribute` — `Name` (optional override)
- `HeaderAttribute` — `Name` (required)
- `BodyAttribute` — request body

## Diagnostic IDs

| ID | Severity | Description |
|----|----------|-------------|
| HTTP001 | Error | HttpClient class must be partial |
| HTTP002 | Error | HTTP method must be partial |
| HTTP003 | Error | HTTP method must not return Task |
| HTTP004 | Error | HTTP path must not be empty |

## Projection Models

- `HttpClientModel` — namespace, typeName, configurationType, baseAddress, requests
- `HttpRequestModel` — verb, methodName, path, returnType, parameters
- `HttpParameterModel` — name, type, kind (Path/Query/Header/Body), customName, defaultValue

## Writers

All in `Deepstaging.Generators/Writers/HttpClient/`:

- `ClientWriter` — generates the partial HTTP client class
- `RequestWriter` — generates request record types
- `InterfaceWriter` — generates the client interface (I{TypeName})
