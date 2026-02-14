# Deepstaging.Generators

Incremental source generator that produces runtime classes and effects modules from Deepstaging attributes.

## Overview

This Roslyn source generator triggers on `[Runtime]` and `[EffectsModule]` attributes, generating capability interfaces,
effects wrapper methods, and dependency injection bootstrapping code.

## Generated Output

For a runtime declaration like:

```csharp
[EffectsModule(typeof(IEmailService), Name = "Email")]
public partial class EmailModule;

[Runtime]
[Uses(typeof(EmailModule))]
public partial class AppRuntime;
```

The generator produces:

- **Capability interfaces** - `IHasEmail` with the `Email` property
- **Effects wrappers** - Methods returning `Eff<RT, T>` for each service method
- **Runtime partial** - Implementation of capability interfaces
- **DI extensions** - `AddAppRuntime()` service collection extension

## Key Class

### DeepstagingGenerator

```csharp
[Generator]
public class DeepstagingGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Registers syntax providers for [Runtime] and [EffectsModule]
        // Transforms symbols into models via Deepstaging.Projection
        // Emits generated source files
    }
}
```

## Dependencies

- `Deepstaging.Projection` - Symbol analysis and model building
- `Deepstaging.Roslyn` - Code emission utilities
- `Deepstaging.Roslyn.Scriban` - Template-based code generation

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Core Attributes](../Deepstaging/README.md)** — `[Runtime]`, `[EffectsModule]`, `[Uses]`
- **[Projection Models](../Deepstaging.Projection/README.md)** — Semantic analysis layer
- **[Analyzers](../Deepstaging.Analyzers/README.md)** — Compile-time validation
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit for queries, projections, and emit
    - [Queries](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Queries.md) — Symbol query
      API
    - [Emit](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Emit.md) — Code generation API
-
    *

*[Deepstaging.Roslyn.Scriban](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Scriban/README.md)
** — Template infrastructure

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
