# Deepstaging.ToDo.Server

Backend API service for the ToDo Aspire application with OpenTelemetry observability.

## Overview

An ASP.NET Core web API that provides weather forecast endpoints and serves as the backend for the ToDo sample application. Includes full observability through OpenTelemetry instrumentation.

## Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/weatherforecast` | Returns 5-day weather forecast |

## Key Files

### Program.cs

Configures the web API:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var app = builder.Build();
app.MapGet("/api/weatherforecast", () => /* forecast data */);
app.Run();
```

### Extensions.cs

Configures Aspire service defaults:

- **OpenTelemetry** - Tracing, metrics, and logging instrumentation
- **Service Discovery** - Automatic service endpoint resolution
- **Resilience** - HTTP client retry and circuit breaker policies
- **Health Checks** - Liveness and readiness endpoints

## Observability

Instrumented with OpenTelemetry packages:
- `OpenTelemetry.Instrumentation.AspNetCore`
- `OpenTelemetry.Instrumentation.Http`
- `OpenTelemetry.Instrumentation.Runtime`

## Running

```bash
# Standalone
dotnet run

# Via Aspire AppHost (recommended)
cd ../Deepstaging.ToDo.AppHost
dotnet run
```

## Related Documentation

- **[Deepstaging README](../../../../README.md)** — Main project overview
- **[AppHost](../Deepstaging.ToDo.AppHost/README.md)** — Aspire orchestration
- **[Frontend](../frontend/README.md)** — React SPA
- **[Domain Library](../../Deepstaging.ToDo/README.md)** — Effects-based domain model
