# Effects Module

The Effects module wraps service interfaces and `DbContext`s as composable [LanguageExt](https://github.com/louthy/language-ext) effect modules with automatic OpenTelemetry instrumentation.

## Overview

```csharp
using Deepstaging.Effects;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

[EffectsModule(typeof(IEmailService), Name = "Email")]
public sealed partial class EmailEffects;

[Runtime]
[Uses(typeof(EmailEffects))]
public sealed partial class AppRuntime;
```

The generator produces:

- **`Eff<RT, A>` wrappers** for each method on the target interface
- **Capability interfaces** (e.g., `IHasEmail`) implemented by the runtime
- **DI registration** extension methods
- **OpenTelemetry `Activity` spans** on each effect invocation (when `Instrumented = true`)

## Attributes

### `[EffectsModule]`

Marks a partial class as an effect module wrapping a target type.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TargetType` | `Type` | *(required)* | The interface or DbContext to wrap |
| `Name` | `string?` | Inferred | Module name (used in capability interface naming) |
| `Instrumented` | `bool` | `true` | Whether to generate OpenTelemetry instrumentation |
| `IncludeOnly` | `string[]?` | `null` | Only wrap these methods |
| `Exclude` | `string[]?` | `null` | Exclude these methods |

### `[Runtime]`

Marks a partial class as the runtime aggregator for effect modules.

### `[Uses]`

Declares that a runtime uses a specific effect module. Repeatable.

| Property | Type | Description |
|----------|------|-------------|
| `ModuleType` | `Type` | The `[EffectsModule]` class to include in the runtime |

## Diagnostics

| ID | Severity | Description |
|----|----------|-------------|
| DSEFX01 | Error | EffectsModule class must be partial |
| DSEFX02 | Warning | EffectsModule class should be sealed |
| DSEFX03 | Warning | EffectsModule target should be an interface |
| DSEFX04 | Error | Duplicate EffectsModule target type |
| DSEFX05 | Error | Excluded method not found on target |
| DSEFX06 | Error | IncludeOnly method not found on target |
| DSEFX07 | Warning | EffectsModule target has no methods |
| DSRT01 | Error | Runtime class must be partial |
| DSRT02 | Error | Uses attribute requires Runtime attribute |
| DSRT03 | Error | Uses target must be an EffectsModule |
| DSRT04 | Info | Effects module available but not referenced by runtime |
