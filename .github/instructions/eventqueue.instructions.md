---
applyTo: src/**/EventQueue/**,src/**/*EventQueue*
---

# EventQueue Module

Namespace: `Deepstaging.EventQueue`

Generates in-process event queues backed by `System.Threading.Channels.Channel<T>` with BackgroundService workers, effect modules, and DI registration.

## Attributes

- `EventQueueAttribute` — `QueueName` (required), `EventBaseType` (optional type constraint), `Capacity` (0=unbounded), `MaxConcurrency` (1=sequential, 0=unlimited), `TimeoutMilliseconds` (0=none), `SingleReader` (default: true), `SingleWriter` (default: false)
- `EventQueueHandlerAttribute<TRuntime>` — `QueueName` (required). Generic `TRuntime` specifies which Runtime the handler targets.

## Diagnostic IDs

| ID | Severity | Description |
|----|----------|-------------|
| DSEQ01 | Error | EventQueue class must be partial |
| DSEQ02 | Warning | EventQueue class should be static |
| DSEQ03 | Error | EventQueueHandler class must be static |
| DSEQ04 | Warning | EventQueueHandler has no handler methods |
| DSEQ05 | Error | EventQueueHandler references unknown queue |

## Runtime Types

All in `Deepstaging.Runtime/EventQueue/`:

- `ChannelWorker<T>` — BackgroundService base with channel loop, concurrency control (SemaphoreSlim), virtual `OnError`, health reporting (depth, rate, error count), graceful shutdown
- `EventQueueChannel<T>` — Typed channel wrapper with `Enqueue`, `EnqueueWithAck`, `EnqueueAndWait`
- `EventAcknowledgement` — TaskCompletionSource wrapper for acknowledgement patterns

## Projection Models

All in `Deepstaging.Projection/EventQueue/Models/`:

- `EventQueueModel` — queueName, containerName, namespace, accessibility, eventBaseType, capacity, maxConcurrency, handlerGroups
- `EventQueueHandlerGroupModel` — handlerType, runtimeType, methods
- `EventHandlerMethodModel` — methodName, eventType, eventTypeName

## Writers

All in `Deepstaging.Generators/Writers/EventQueue/`:

- `EventQueueEffectModuleWriter` — emits static partial class with `Enqueue`, `EnqueueWithAck`, `EnqueueAndWait` effect methods delegating to `EventQueueChannel<T>`
