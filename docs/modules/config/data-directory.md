# Data Directory

All Deepstaging configuration files live in a single directory relative to your project root. This page explains how to control that location.

## Default: `.config/`

Out of the box, files are generated under `.config/`:

```
MyProject/
├── MyProject.csproj
└── .config/
    ├── deepstaging.props
    ├── deepstaging.targets
    ├── deepstaging.schema.json
    ├── deepstaging.settings.json
    └── ...
```

## Customizing the Location

Set the `DeepstagingDataDirectory` MSBuild property in your `.csproj`:

```xml
<PropertyGroup>
    <DeepstagingDataDirectory>config</DeepstagingDataDirectory>
</PropertyGroup>
```

Or in a `Directory.Build.props` to apply across all projects:

```xml
<PropertyGroup>
    <DeepstagingDataDirectory>build-config</DeepstagingDataDirectory>
</PropertyGroup>
```

This is the **single source of truth**. The property flows to:

- NuGet-provided MSBuild imports (where to find local props/targets)
- Analyzers (where to look for existing schemas)
- Code fixes (where to write generated files)

## How It Works

The `DeepstagingDataDirectory` property is declared as a `<CompilerVisibleProperty>` in the NuGet package's `build/Deepstaging.props`. This makes it available to Roslyn analyzers at compile time.

When an analyzer reports a diagnostic (e.g., DSCFG06 "configuration files missing"), it includes the data directory value in the diagnostic's properties. The code fix reads this value to determine where to create files.

See [MSBuild Integration](../concepts/msbuild-integration.md) for the full architecture.

## Gotcha: Moving an Existing Data Directory

If you change `DeepstagingDataDirectory` after files have been generated, the old files remain in place. You'll need to:

1. Move the files manually to the new location
2. Rebuild to clear stale diagnostics

The NuGet imports use conditional `Exists()` checks, so orphaned files in the old location won't cause build errors — they just won't be imported.
