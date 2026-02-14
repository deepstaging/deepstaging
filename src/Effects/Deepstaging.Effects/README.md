# Deepstaging

Core attributes for marking classes for Deepstaging code generation.

## Overview

This package provides the attribute definitions that drive the Deepstaging source generators. Apply these attributes to
partial classes to generate effects-based runtime infrastructure.

## Attributes

### RuntimeAttribute

Marks a partial class as the effects runtime entry point:

```csharp
[Runtime]
public partial class AppRuntime;
```

### EffectsModuleAttribute

Wraps a target type (interface or DbContext) into an effects module:

```csharp
[EffectsModule(typeof(IEmailService), Name = "Email")]
public partial class EmailModule;
```

### UsesAttribute

Declares module dependencies within a runtime:

```csharp
[Runtime]
[Uses(typeof(EmailModule))]
[Uses(typeof(StorageModule))]
public partial class AppRuntime;
```

## Complete Example

```csharp
// Define effects modules
[EffectsModule(typeof(IEmailService), Name = "Email")]
public partial class EmailModule;

[EffectsModule(typeof(IStorageService), Name = "Storage")]
public partial class StorageModule;

// Compose into runtime
[Runtime]
[Uses(typeof(EmailModule))]
[Uses(typeof(StorageModule))]
public partial class AppRuntime;
```

The source generators will produce:

- Capability interfaces for each module
- Effects wrapper methods
- DI registration extensions

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Source Generators](../Deepstaging.Generators/README.md)** — How code is generated
- **[Analyzers](../Deepstaging.Analyzers/README.md)** — Compile-time validation
- **[Runtime Utilities](../Deepstaging.Runtime/README.md)** — OpenTelemetry and EF Core support

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
