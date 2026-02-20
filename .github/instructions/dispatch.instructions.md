---
applyTo: src/**/Dispatch/**,src/**/*Dispatch*
---

# Dispatch Module

Namespace: `Deepstaging.Dispatch`

Generates strongly-typed dispatch method overloads as direct LanguageExt `Eff` compositions — one per command/query handler. No runtime routing or lookup occurs; method overload resolution IS the dispatch.

## Attributes

- `DispatchModuleAttribute` — `AutoCommit` (default: true). Marks a static partial class as the dispatch module.
- `CommandHandlerAttribute` — Marks a static class as containing command handler methods.
- `QueryHandlerAttribute` — Marks a static class as containing query handler methods.

## Marker Interfaces

- `ICommand` — Marker for command types (intent to change state).
- `IQuery<TResult>` — Marker for query types (intent to read state).
- `IAutoCommittable` — Interface with `CommitAsync(CancellationToken)` for auto-commit participants.

## Supporting Types

- `QueryResult<T>` — Sealed record with paging metadata: `Data`, `TotalCount`, `Page`, `PageSize`, `TotalPages`, `HasNextPage`, `HasPreviousPage`.

## Diagnostic IDs

| ID | Severity | Description |
|----|----------|-------------|
| DSDSP01 | Error | DispatchModule class must be partial |
| DSDSP02 | Error | DispatchModule class must be static |
| DSDSP03 | Error | CommandHandler class must be static |
| DSDSP04 | Error | QueryHandler class must be static |
| DSDSP05 | Warning | CommandHandler has no handler methods |
| DSDSP06 | Warning | QueryHandler has no handler methods |

## Projection Models

All in `Deepstaging.Projection/Dispatch/Models/`:

- `DispatchModel` — containerName, namespace, accessibility, autoCommit, commandHandlers, queryHandlers
- `DispatchHandlerGroupModel` — handlerType, methods
- `DispatchHandlerMethodModel` — methodName, inputType, resultType, runtimeType

## Writers

All in `Deepstaging.Generators/Writers/Dispatch/`:

- `DispatchModuleWriter` — emits direct `from/select` Eff compositions per handler with optional auto-commit via `IAutoCommittable`

## Design

The generator knows at compile time which handler handles which command. It emits a direct overload per command type:

```csharp
public static Eff<AppRuntime, OrderCreated> Dispatch(CreateOrder command) =>
    from result in OrderCommands.Handle(command)
    from _ in liftEff<AppRuntime, Unit>(async rt => { if (rt is IAutoCommittable c) await c.CommitAsync(default); return unit; })
    select result;
```

Method overload resolution IS the dispatch. Zero indirection.
