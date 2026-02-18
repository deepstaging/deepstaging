# Deepstaging Config

Strongly-typed configuration providers with source-generated interfaces, DI registration, and JSON Schema output.

> **See also:** [Main README](../../README.md) | [Effects System](../README.md)

## Quick Start

### 1. Define Your Configuration Types

```csharp
public class SlackConfig
{
    public string WebhookUrl { get; init; } = "";
    public int RetryCount { get; init; }
}

public class SlackSecretsConfig
{
    [Secret]
    public string ApiToken { get; init; } = "";
}
```

### 2. Create a Config Provider

```csharp
using Deepstaging.Config;

[ConfigProvider]
[Exposes<SlackConfig>]
[Exposes<SlackSecretsConfig>]
public sealed partial class SlackConfigProvider;
```

### 3. Configure Sources & Register in DI

```csharp
// Add configuration sources (deepstaging.settings.json + user secrets if applicable)
builder.Configuration.ConfigureSlackConfigProviderSources();

// Register the provider
builder.Services.AddSlackConfigProvider(builder.Configuration);
```

### 4. Inject the Interface

```csharp
public class MyService(ISlackConfigProvider config)
{
    public void Send()
    {
        var url = config.SlackConfig.WebhookUrl;
        var token = config.SlackSecretsConfig.ApiToken;
    }
}
```

## Attributes

### [ConfigProvider]

Marks a partial class as a configuration provider. The generator creates:

- An **interface** (`ISlackConfigProvider`) with typed properties
- A **partial class** implementation bound to `IConfiguration`
- A **DI extension** method (`services.AddSlackConfigProvider(configuration)`)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Section` | `string?` | Inferred | Configuration section name |

**Section inference:** When `Section` is omitted, it is inferred by stripping the `ConfigProvider` suffix from the class name. For example, `SlackConfigProvider` → `"Slack"`. If the class name does not end in `ConfigProvider`, use an explicit section or diagnostic DSCFG03 fires.

### [Exposes\<T\>]

Declares a configuration type to expose. Can be applied multiple times. The generator introspects `T` for its public instance properties and generates a typed accessor on the provider.

```csharp
[ConfigProvider(Section = "Notifications")]
[Exposes<EmailConfig>]
[Exposes<SlackConfig>]
public sealed partial class NotificationsConfigProvider;
```

### [Secret]

Marks a configuration property as sensitive (passwords, API keys, tokens, connection strings). Properties decorated with `[Secret]` are:

- **Excluded** from `deepstaging.schema.json`
- **Included** in a separate `user-secrets.schema.json`

```csharp
public class DbConfig
{
    public string Host { get; init; } = "";

    [Secret]
    public string ConnectionString { get; init; } = "";
}
```

## Generated Output

Given the quick start example above, the generator produces:

### Interface

```csharp
public interface ISlackConfigProvider
{
    SlackConfig SlackConfig { get; set; }
    SlackSecretsConfig SlackSecretsConfig { get; set; }
}
```

### Partial Class

```csharp
public sealed partial class SlackConfigProvider : ISlackConfigProvider
{
    private readonly IConfigurationSection _section;

    public SlackConfigProvider(IConfiguration configuration)
    {
        _section = configuration.GetSection("Slack");
    }

    public SlackConfig SlackConfig => _section.GetSection("SlackConfig").Get<SlackConfig>()!;
    public SlackSecretsConfig SlackSecretsConfig => _section.GetSection("SlackSecretsConfig").Get<SlackSecretsConfig>()!;
}
```

### DI Extension

```csharp
public static class SlackConfigProviderExtensions
{
    public static IConfigurationBuilder ConfigureSlackConfigProviderSources(
        this IConfigurationBuilder builder,
        string? settingsPath = null)
    {
        builder.AddJsonFile(settingsPath ?? "deepstaging.settings.json", optional: true, reloadOnChange: true);
        builder.AddUserSecrets(typeof(SlackConfigProvider).Assembly, optional: true);
        return builder;
    }

    public static IServiceCollection AddSlackConfigProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = new SlackConfigProvider(configuration);
        services.AddSingleton<ISlackConfigProvider>(provider);
        return services;
    }
}
```

> **Note:** `AddUserSecrets` is only generated when `[Secret]` properties are present. It requires a `<UserSecretsId>` in the `.csproj`. Run `dotnet user-secrets init` to add one. Analyzer DSCFG07 warns if this is missing.

## JSON Schema

The analyzer produces two schema files via code fixes:

- **`deepstaging.schema.json`** — all non-secret properties
- **`user-secrets.schema.json`** — only `[Secret]`-marked properties

These follow [JSON Schema Draft-7](https://json-schema.org/draft-07/schema#) and can be referenced from your configuration files for IDE autocompletion.

## Analyzers

| ID | Severity | Description | Code Fix |
|----|----------|-------------|----------|
| DSCFG01 | Error | ConfigProvider class must be partial | Add `partial` modifier |
| DSCFG02 | Warning | ConfigProvider class should be sealed | Add `sealed` modifier |
| DSCFG03 | Error | Section name could not be inferred | — |
| DSCFG04 | Warning | Exposed type has no public properties | — |
| DSCFG05 | Warning | Property appears to contain secrets/PII | Add `[Secret]` attribute |
| DSCFG06 | Info | Schema files can be generated | Generate schema files |
| DSCFG07 | Warning | [Secret] properties exist but no UserSecretsId | — |

### DSCFG05 — Secret Heuristics

The analyzer flags properties whose names match common secret/PII patterns:

`Password`, `Secret`, `Token`, `ApiKey`, `ConnectionString`, `Credential`, `PrivateKey`, `AccessKey`, `ClientSecret`

## Configuration Layout

The expected `appsettings.json` structure mirrors the provider hierarchy:

```json
{
  "Slack": {
    "SlackConfig": {
      "WebhookUrl": "https://hooks.slack.com/...",
      "RetryCount": 3
    },
    "SlackSecretsConfig": {
      "ApiToken": "xoxb-..."
    }
  }
}
```

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

See [LICENSE](../../../LICENSE) for the full legal text.
