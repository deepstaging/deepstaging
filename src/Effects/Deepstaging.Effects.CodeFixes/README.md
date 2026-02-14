# Deepstaging.CodeFixes

Automatic code fixes for Deepstaging analyzer diagnostics.

## Overview

Provides CodeFixProviders that offer quick fixes in the IDE for violations detected by `Deepstaging.Analyzers`.

## Code Fixes

| Diagnostic                              | Fix                               |
|-----------------------------------------|-----------------------------------|
| DS0001 - EffectsModule must be partial  | Adds `partial` modifier           |
| DS0002 - Runtime must be partial        | Adds `partial` modifier           |
| DS0003 - EffectsModule should be sealed | Adds `sealed` modifier            |
| DS0008 - Uses must target runtime       | Suggests correct attribute target |

## CodeFix Classes

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp)]
public class EffectsModuleMustBePartialCodeFix : CodeFixProvider
{
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        // Registers action to add 'partial' modifier
    }
}
```

## Usage

Code fixes appear automatically in IDEs (Visual Studio, Rider, VS Code) when the corresponding analyzer diagnostic is
triggered. Use `Ctrl+.` or the lightbulb menu to apply fixes.

### Before Fix

```csharp
[EffectsModule(typeof(IEmailService))]
public class EmailModule;  // Squiggle: DS0001
```

### After Fix

```csharp
[EffectsModule(typeof(IEmailService))]
public partial class EmailModule;  // Fixed
```

## Dependencies

- `Deepstaging.Roslyn` - Symbol analysis utilities
- `Deepstaging.Roslyn.Workspace` - Code fix infrastructure

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Analyzers](../Deepstaging.Analyzers/README.md)** — Diagnostics that trigger these fixes
- **[Core Attributes](../Deepstaging/README.md)** — Attribute definitions
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit
- *
  *[Deepstaging.Roslyn.Workspace](https://github.com/deepstaging/roslyn/blob/main/src/Deepstaging.Roslyn.Workspace/README.md)
  ** — Code fix provider infrastructure

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
