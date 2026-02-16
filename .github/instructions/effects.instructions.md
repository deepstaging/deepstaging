---
applyTo: src/**/Effects/**,src/**/*Effects*
---

# Effects Module

Namespace: `Deepstaging.Effects`

Wraps interfaces and DbContexts as LanguageExt effect modules with OpenTelemetry instrumentation.

## Attributes

- `EffectsModuleAttribute` — `TargetType` (required), `Name`, `Instrumented` (default: true), `IncludeOnly`, `Exclude`
- `RuntimeAttribute` — marks a partial class as the runtime aggregator
- `UsesAttribute` — declares which effect modules a runtime uses (`ModuleType`)

## Diagnostic IDs

| ID | Severity | Description |
|----|----------|-------------|
| DS0001 | Error | EffectsModule class must be partial |
| DS0002 | Error | Runtime class must be partial |
| DS0003 | Error | Uses attribute requires Runtime attribute |
| DS0004 | Error | EffectsModule target should be an interface |
| DS0005 | Error | Duplicate EffectsModule target type |
| DS0006 | Error | Excluded method not found on target |
| DS0007 | Error | IncludeOnly method not found on target |
| DS0008 | Error | Uses target must be an EffectsModule |
| DS0009 | Warning | EffectsModule class should be sealed |

## Projection Models

- `EffectsModuleModel` — module name, target type, methods, instrumented, isDbContext, dbSets
- `RuntimeModel` — runtime type, capabilities, activity sources
- `EffectMethodModel` / `EffectParameterModel` — individual effect methods and params
- `DbSetModel` — DbSet property info for DbContext modules

## Writers

All in `Deepstaging.Generators/Writers/Effects/`:

- `EffectsModuleWriter` (+ `.Methods`, `.DbContext` partials) — emits the effects module class
- `DbSetQueryWriter` — DbSet query builder generation
- `RuntimeWriter` — emits the partial runtime with capability interfaces
- `RuntimeBootstrapperWriter` — DI registration code
