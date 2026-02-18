# Deepstaging Effects System

Source-generated effect wrappers for C#/.NET services with OpenTelemetry instrumentation.

> **See also:** [Main README](../README.md) | [Roslyn Toolkit](../roslyn/README.md)

## Packages

| Package | Description |
|---------|-------------|
| **Deepstaging** | Marker attributes (`[EffectsModule]`, `[Runtime]`, `[Uses]`) |
| **Deepstaging.Generators** | Source generators that produce effect wrappers |
| **Deepstaging.Analyzers** | Diagnostic analyzers for correct usage |
| **Deepstaging.CodeFixes** | Quick fixes for analyzer diagnostics |
| **Deepstaging.Runtime** | Runtime support (activity extensions, metrics) |
| **Deepstaging.Projection** | Projection utilities |

## How It Works

### 1. Define Your Services

```csharp
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

public interface ISlackService
{
    Task PostMessageAsync(string channel, string message);
}
```

### 2. Create an Effects Module

```csharp
using Deepstaging;

[EffectsModule(typeof(IEmailService), Name = "Email")]
[EffectsModule(typeof(ISlackService), Name = "Slack")]
public partial class AppEffects;
```

### 3. Use Generated Effects

The generator creates effect wrappers you can compose:

```csharp
var program = 
    from _ in AppEffects.Email.SendAsync("user@example.com", "Hello", "World")
    from __ in AppEffects.Slack.PostMessageAsync("#general", "Email sent!")
    select unit;
```

## Attributes

### [EffectsModule]

Wraps a service interface as composable effects:

```csharp
[EffectsModule(typeof(IEmailService))]              // Basic usage
[EffectsModule(typeof(IEmailService), Name = "Email")]  // Custom name
[EffectsModule(typeof(IEmailService), Instrumented = false)]  // No telemetry
[EffectsModule(typeof(IEmailService), IncludeOnly = ["SendAsync"])]  // Whitelist
[EffectsModule(typeof(IEmailService), Exclude = ["Ping"])]  // Blacklist
```

| Property | Description |
|----------|-------------|
| `TargetType` | The interface to wrap (required) |
| `Name` | Module name (defaults to interface name without `I` prefix) |
| `Instrumented` | Enable OpenTelemetry spans (default: `true`) |
| `IncludeOnly` | Only wrap these methods |
| `Exclude` | Exclude these methods |

### [Runtime]

Defines an effects runtime that composes multiple modules:

```csharp
[Runtime]
public sealed partial class AppRuntime;
```

### [Uses]

Declares which effects modules a runtime uses:

```csharp
[Runtime]
[Uses(typeof(AppEffects))]
public sealed partial class AppRuntime;
```

## Analyzers

The analyzers catch common mistakes at compile time:

| Diagnostic | Description |
|------------|-------------|
| `DSRT01` | Runtime must be partial |
| `DSRT02` | Uses attribute must target a runtime |
| `DSRT03` | Uses target must be an effects module |
| `DSRT04` | Effects module available but not referenced |
| `DSEFX01` | Effects module must be partial |
| `DSEFX02` | Effects module should be sealed |
| `DSEFX03` | Target type must be an interface |
| `DSEFX04` | Duplicate target type in module |
| `DSEFX05` | Exclude method not found on target |
| `DSEFX06` | IncludeOnly method not found on target |
| `DSEFX07` | Effects module target has no methods |

## OpenTelemetry Integration

When `Instrumented = true` (the default), generated effects automatically create OpenTelemetry spans:

```csharp
// Generated code includes:
.WithActivity(ActivitySource, "Email.SendAsync")
```

- Zero overhead when no `ActivityListener` is registered
- Automatic span creation with service/method names
- Integrates with your existing telemetry pipeline

## Project Structure

```
src/
├── Deepstaging/           # Marker attributes (entry point for consumers)
├── Deepstaging.Generators/ # Source generators
│   ├── DeepstagingGenerator.cs
│   └── Writers/           # Template invocation
├── Deepstaging.Analyzers/  # Diagnostic analyzers
├── Deepstaging.CodeFixes/  # Quick fixes
├── Deepstaging.Runtime/    # Runtime support
└── Deepstaging.Projection/ # Projection utilities
```

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../LICENSE) for the full legal text.
