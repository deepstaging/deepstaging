# Getting Started

## Installation

```bash
dotnet add package Deepstaging --prerelease
```

This adds the source generators, analyzers, and code fixes to your project. No runtime dependency is required for most features.

For Effects modules with OpenTelemetry instrumentation, also add:

```bash
dotnet add package Deepstaging.Runtime --prerelease
```

## Your First Typed ID

```csharp
using Deepstaging.Ids;

[TypedId]
public readonly partial struct UserId;
```

Build, and the generator creates a complete `UserId` struct with `IEquatable<UserId>`, `IParsable<UserId>`, `IComparable<UserId>`, `ToString()`, and a factory method.

```csharp
var id = UserId.New();          // New random GUID-backed ID
var parsed = UserId.Parse("...");
```

## Your First Config Provider

```csharp
using Deepstaging.Config;

public sealed record DatabaseConfig
{
    public required string ConnectionString { get; init; }
    public int MaxRetries { get; init; } = 3;
}

[ConfigProvider]
[Exposes<DatabaseConfig>]
public sealed partial class DatabaseConfigProvider;
```

The generator creates:

- An `IDatabaseConfigProvider` interface
- A partial implementation that binds to `IConfiguration` section `"Database"` (inferred from class name)
- A DI extension method: `services.AddDatabaseConfigProvider(configuration)`

### Configuration Files

When you build, analyzer **DSCFG06** will suggest generating configuration files. Apply the code fix to create:

```
.config/
├── deepstaging.props              # MSBuild settings for this project
├── deepstaging.targets            # File nesting for IDE organization
├── deepstaging.schema.json        # JSON Schema for validation
├── deepstaging.settings.json      # Settings template
├── deepstaging.settings.Development.json
├── deepstaging.settings.Staging.json
└── deepstaging.settings.Production.json
```

See [Data Directory](modules/config/data-directory.md) for details on customizing the file location.

## Your First Effects Module

```csharp
using Deepstaging.Effects;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

[EffectsModule(typeof(IEmailService))]
public sealed partial class EmailEffects;

[Runtime]
[Uses(typeof(EmailEffects))]
public sealed partial class AppRuntime;
```

The generator wraps each method as an `Eff<RT, A>` with OpenTelemetry `Activity` spans, and wires everything through the runtime's capability interfaces.

## Analyzers

Every module ships with analyzers that catch mistakes at compile time. If you forget `partial`, target the wrong type, or make a configuration error — you'll see a compiler error, not a runtime exception.

See the [Diagnostics Reference](diagnostics.md) for the complete list.
