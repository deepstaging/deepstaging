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

### Runtime Diagnostics (DSRT)

| ID | Severity | Description |
|----|----------|-------------|
| DSRT01 | Error | Runtime class must be partial |
| DSRT02 | Error | Uses attribute requires Runtime attribute |
| DSRT03 | Error | Uses target must be an EffectsModule |
| DSRT04 | Info | Effects module available but not referenced by runtime |

### EffectsModule Diagnostics (DSEFX)

| ID | Severity | Description |
|----|----------|-------------|
| DSEFX01 | Error | EffectsModule class must be partial |
| DSEFX02 | Warning | EffectsModule class should be sealed |
| DSEFX03 | Warning | EffectsModule target should be an interface |
| DSEFX04 | Error | Duplicate EffectsModule target type |
| DSEFX05 | Error | Excluded method not found on target |
| DSEFX06 | Error | IncludeOnly method not found on target |
| DSEFX07 | Warning | EffectsModule target has no methods |

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
