# Deepstaging.Analyzers

Roslyn analyzers that enforce Deepstaging best practices and catch configuration errors at compile time.

## Overview

Provides diagnostic analyzers that validate `[Runtime]` and `[EffectsModule]` attribute usage, ensuring correct
declarations before source generation runs.

## Diagnostics

| ID     | Severity | Description                                                |
|--------|----------|------------------------------------------------------------|
| DS0001 | Error    | `[EffectsModule]` class must be partial                    |
| DS0002 | Error    | `[Runtime]` class must be partial                          |
| DS0003 | Warning  | `[EffectsModule]` class should be sealed                   |
| DS0004 | Error    | `[EffectsModule]` target must be an interface or DbContext |
| DS0005 | Error    | `IncludeOnly` method not found on target type              |
| DS0006 | Error    | `Exclude` method not found on target type                  |
| DS0007 | Error    | Duplicate target type in effects modules                   |
| DS0008 | Error    | `[Uses]` attribute must target a runtime class             |

## Analyzer Classes

```csharp
// Ensures effects modules are partial for source generation
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EffectsModuleMustBePartialAnalyzer : DiagnosticAnalyzer

// Ensures runtime classes are partial for source generation
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RuntimeMustBePartialAnalyzer : DiagnosticAnalyzer
```

## Example Error

```csharp
// DS0001: EffectsModule 'EmailModule' must be declared as partial
[EffectsModule(typeof(IEmailService))]
public class EmailModule;  // ❌ Missing 'partial'
```

Fix:

```csharp
[EffectsModule(typeof(IEmailService))]
public partial class EmailModule;  // ✅
```

## Dependencies

- `Deepstaging.Roslyn` - Symbol analysis utilities

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Code Fixes](../Deepstaging.CodeFixes/README.md)** — Automatic fixes for these diagnostics
- **[Core Attributes](../Deepstaging/README.md)** — Attribute definitions being validated
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit
    - [Queries](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn/Docs/Queries.md) — Symbol query
      API

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
