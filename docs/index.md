# Deepstaging

Source generators for strongly-typed IDs, configuration, effects modules, and HTTP clients.

You annotate your types with attributes. The generators, analyzers, and code fixes do the rest — all at compile time, with zero reflection.

## Quick Start

```bash
dotnet add package Deepstaging --prerelease
```

## Modules

| Module | What it does |
|--------|-------------|
| [Effects](modules/effects/index.md) | Wraps interfaces and DbContexts as LanguageExt effect modules with OpenTelemetry |
| [Typed IDs](modules/ids/index.md) | Type-safe ID structs with configurable backing types and converters |
| [Configuration](modules/config/index.md) | Strongly-typed config providers with JSON Schema generation |
| [HTTP Client](modules/httpclient/index.md) | Declarative HTTP clients from annotated partial classes |

## How It Works

Each module follows the same pattern:

1. **You** add an attribute to a `partial` type
2. **Generators** emit the implementation at compile time
3. **Analyzers** catch mistakes as compiler errors (missing `partial`, wrong target type, etc.)
4. **Code fixes** offer one-click repairs

```csharp
[TypedId]
public readonly partial struct UserId;

[ConfigProvider]
[Exposes<DatabaseConfig>]
public sealed partial class DatabaseConfigProvider;

[EffectsModule(typeof(IEmailService))]
public sealed partial class EmailEffects;

[HttpClient<ApiConfig>]
public partial class UsersClient
{
    [Get("/users/{id}")]
    private partial User GetUser(int id);
}
```

## Concepts

- [MSBuild Integration](concepts/msbuild-integration.md) — How Deepstaging integrates with your build via NuGet props/targets and local configuration files
- [Diagnostics Reference](diagnostics.md) — All analyzer diagnostic IDs

## Built On

[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn) — a fluent toolkit for building Roslyn source generators, analyzers, and code fixes.

## License

**RPL-1.5** (Reciprocal Public License)
