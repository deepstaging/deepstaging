# Deepstaging.ToDo.AppHost

Aspire orchestration host that coordinates the distributed ToDo application.

## Overview

Defines the application topology, manages service dependencies, and orchestrates startup order for the Server and Frontend components.

## Key Files

### AppHost.cs

Registers and wires up application components:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.Deepstaging_ToDo_Server>("server")
    .WithHttpHealthCheck("/health");

builder.AddViteApp("frontend", "../frontend")
    .WithReference(server)
    .PublishAsDockerFile();

builder.Build().Run();
```

## Application Topology

```
┌─────────────────┐
│    AppHost      │
│  (Orchestrator) │
└────────┬────────┘
         │
    ┌────┴────┐
    │         │
    ▼         ▼
┌───────┐ ┌──────────┐
│Server │◄│ Frontend │
└───────┘ └──────────┘
```

## Features

- **Health Monitoring** - HTTP health checks on server endpoints
- **Service Discovery** - Frontend automatically discovers server URL
- **Dependency Ordering** - Frontend waits for server to be healthy
- **Container Publishing** - Frontend publishes as Docker image

## Running

```bash
dotnet run
```

The Aspire dashboard will open at `https://localhost:15000` showing all orchestrated services.

## Related Documentation

- **[Deepstaging README](../../../../README.md)** — Main project overview
- **[Server](../Deepstaging.ToDo.Server/README.md)** — Backend API service
- **[Frontend](../frontend/README.md)** — React SPA
- **[Domain Library](../../Deepstaging.ToDo/README.md)** — Effects-based domain model
