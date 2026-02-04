# Deepstaging

Turn your service interfaces into composable, instrumented effects with zero boilerplate.

## Installation

```bash
dotnet add package Deepstaging
```

### Quick Start

**1. Define your services**

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

**2. Create an effects module**

```csharp
using Deepstaging;

[EffectsModule(typeof(IEmailService), Name = "Email")]
[EffectsModule(typeof(ISlackService), Name = "Slack")]
public partial class AppEffects;
```

**3. Define your runtime**

```csharp
[Runtime]
[Uses(typeof(AppEffects))]
public partial class AppRuntime;
```

**4. Compose and run effects**

```csharp
using static AppEffects;

// Compose effects with LINQ-style syntax
var workflow = 
    from _ in Email.Send("user@example.com", "Hello", "Welcome!")
    from _ in Slack.PostMessage("#general", "New user signed up!")
    select unit;

// Run with your runtime
await workflow.Run(runtime);
```

### Features

| Feature | Description |
|---------|-------------|
| **Zero Reflection** | Compile-time source generation for maximum performance |
| **OpenTelemetry** | Built-in tracing with `Activity` spans (zero overhead when disabled) |
| **LanguageExt** | First-class `Eff<RT, A>` integration for functional composition |
| **Analyzers** | Catch configuration errors at compile time |
| **Code Fixes** | Quick fixes for common mistakes |

### Configuration Options

```csharp
[EffectsModule(
    typeof(IEmailService),
    Name = "Email",              // Module name (default: derived from type)
    Instrumented = true,         // Enable OpenTelemetry tracing (default: true)
    IncludeOnly = ["SendAsync"], // Only wrap these methods
    Exclude = ["Ping"]           // Exclude these methods
)]
public partial class EmailModule;
```

## Project Structure

```
src/
├── Deepstaging              # Core attributes (EffectsModule, Runtime, Uses)
├── Deepstaging.Generators   # Source generators for effects
├── Deepstaging.Analyzers    # Diagnostic analyzers
├── Deepstaging.CodeFixes    # Code fix providers
├── Deepstaging.Runtime      # Runtime support (OpenTelemetry, EF Core)
├── Deepstaging.Projection   # Query models for Deepstaging attributes
```

## Related Projects

- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Fluent toolkit for building Roslyn source generators, analyzers, and code fixes

## Build & Test

```bash
# Build
dotnet build Deepstaging.slnx

# Test
dotnet test Deepstaging.slnx

# Run specific test
dotnet test --filter "FullyQualifiedName~MyTestClass.MyTestMethod"
```

## Documentation

- **[Build Configuration](build/README.md)** - MSBuild configuration files

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](LICENSE) for the full legal text.
