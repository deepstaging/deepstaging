# Deepstaging.ToDo

A sample domain library demonstrating Deepstaging's effects-based architecture for a Todo application.

## Overview

This project showcases how to compose multiple service dependencies using Deepstaging's `EffectsModule` and `Runtime` attributes. The source generators automatically wire up the effects orchestration at compile time.

## Key Components

### ToDoEffects

Composes three service modules into a single effects container:

```csharp
[EffectsModule(typeof(IEmailService), Name = "Email")]
[EffectsModule(typeof(ISlackService), Name = "Slack")]
[EffectsModule(typeof(TodoDbContext), Name = "Database")]
public partial class ToDoEffects;
```

### ToDoRuntime

The runtime orchestrator that uses the composed effects:

```csharp
[Runtime]
[Uses(typeof(ToDoEffects))]
public partial class ToDoRuntime;
```

## Services

| Service | Description |
|---------|-------------|
| `IEmailService` | Email notification interface |
| `ISlackService` | Slack notification interface |
| `TodoDbContext` | EF Core database context with `TodoItems` |

## Usage

```csharp
// The runtime provides access to all composed effects
var runtime = new ToDoRuntime();

// Access effects via generated properties
runtime.Email.SendAsync(...);
runtime.Slack.NotifyAsync(...);
runtime.Database.TodoItems.Add(...);
```

## Project Structure

```
Deepstaging.ToDo/
├── ToDoRuntime.cs      # Runtime and effects definitions
├── Services/           # Service interfaces and implementations
└── Generated/          # Compiler-generated effects code
```

## Related Documentation

- **[Deepstaging README](../../../README.md)** — Main project overview and quick start
- **[Core Attributes](../../../src/Deepstaging/README.md)** — `[Runtime]`, `[EffectsModule]`, `[Uses]`
- **[Server](../AspireApp1/Deepstaging.ToDo.Server/README.md)** — Backend API service
- **[AppHost](../AspireApp1/Deepstaging.ToDo.AppHost/README.md)** — Aspire orchestration
