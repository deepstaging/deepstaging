# Deepstaging.Runtime

Runtime utilities for LanguageExt effects, including OpenTelemetry instrumentation and EF Core query builders.

## Overview

Provides infrastructure for executing effects with observability (tracing, metrics, logging) and fluent database query APIs that return composable `Eff<RT, T>` effects.

## Key Components

### DbSetQuery

Fluent LINQ query builder returning composable effects:

```csharp
var users = await query
    .Where(x => x.Active)
    .OrderBy(x => x.Name)
    .Take(10)
    .ToListAsync()
    .Run(runtime);
```

### ActivityEffectExtensions

Wraps effects with OpenTelemetry spans for distributed tracing:

```csharp
effect.WithActivity("AppRuntime.Email.SendAsync", activitySource, logger);
```

### EffectMetrics

OpenTelemetry metrics collection for success/failure tracking:

- Counters for effect invocations
- Histograms for duration tracking
- Success/failure labeling

## Classes

| Class | Description |
|-------|-------------|
| `DbSetQuery<RT, T>` | Fluent query builder for EF Core DbSets |
| `DbSetOrderedQuery<RT, T>` | Ordered query continuation |
| `ActivityEffectExtensions` | OpenTelemetry activity tracing |
| `EffectMetrics` | Metrics instrumentation |

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Core Attributes](../Deepstaging/README.md)** — `[Runtime]`, `[EffectsModule]`, `[Uses]`
- **[Source Generators](../Deepstaging.Generators/README.md)** — Code generation details

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
