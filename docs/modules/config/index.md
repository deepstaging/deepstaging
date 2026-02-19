# Configuration Module

The Configuration module generates strongly-typed configuration providers with interfaces, DI registration, and JSON Schema output.

## Overview

```csharp
using Deepstaging.Config;

public sealed record SmtpSettings(string Host, int Port);

public class SmtpSecrets
{
    [Secret]
    public string Password { get; init; } = "";
}

[ConfigProvider(Section = "Smtp")]
[Exposes<SmtpSettings>]
[Exposes<SmtpSecrets>]
public sealed partial class SmtpConfigProvider;
```

The generator produces:

- **`ISmtpConfigProvider`** — interface exposing the configuration properties
- **Partial class implementation** — binds properties from `IConfiguration["Smtp"]`
- **DI extension method** — `services.AddSmtpConfigProvider(configuration)`

## Attributes

### `[ConfigProvider]`

Marks a partial class as a configuration provider.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Section` | `string?` | Inferred | Configuration section name. If omitted, inferred by stripping `ConfigProvider` suffix from the class name. |

```csharp
// Section = "Smtp" (explicit)
[ConfigProvider(Section = "Smtp")]
public sealed partial class SmtpConfigProvider;

// Section = "Database" (inferred from "DatabaseConfigProvider")
[ConfigProvider]
public sealed partial class DatabaseConfigProvider;
```

### `[Exposes<T>]`

Declares a configuration type to expose from the provider. Repeatable.

```csharp
[ConfigProvider]
[Exposes<DatabaseConfig>]
[Exposes<DatabaseSecrets>]
public sealed partial class DatabaseConfigProvider;
```

### `[Secret]`

Marks a property as sensitive. Secret properties are:

- **Excluded** from the main `deepstaging.schema.json`
- **Included** in a separate `user-secrets.schema.json`
- Backed by .NET User Secrets when `UserSecretsId` is configured

```csharp
public class SmtpSecrets
{
    [Secret]
    public string Password { get; init; } = "";
    
    [Secret]
    public string ApiKey { get; init; } = "";
}
```

## Generated Files

When you apply the DSCFG06 code fix, the following files are created in your [data directory](data-directory.md) (default `.config/`):

| File | Purpose |
|------|---------|
| `deepstaging.props` | MSBuild project properties |
| `deepstaging.targets` | File nesting metadata for IDEs |
| `deepstaging.schema.json` | JSON Schema for non-secret configuration |
| `deepstaging.settings.json` | Settings template |
| `deepstaging.settings.{Environment}.json` | Per-environment overrides |
| `user-secrets.schema.json` | JSON Schema for secret properties |
| `user-secrets.json` | Local secrets file (gitignored) |

## Diagnostics

| ID | Severity | Description |
|----|----------|-------------|
| DSCFG01 | Error | ConfigProvider class must be partial |
| DSCFG02 | Warning | ConfigProvider class should be sealed |
| DSCFG03 | Error | Section name could not be inferred |
| DSCFG04 | Warning | Exposed type has no public instance properties |
| DSCFG05 | Warning | Property appears to contain secrets — consider adding `[Secret]` |
| DSCFG06 | Info/Warning | Configuration files missing or out of date |
| DSCFG07 | Error | `[Secret]` properties exist but assembly has no `UserSecretsId` |
