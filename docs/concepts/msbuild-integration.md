# MSBuild Integration

Deepstaging integrates with your build through a three-layer system of MSBuild files. Understanding this architecture helps when customizing behavior or troubleshooting.

## The Three Layers

### Layer 1: NuGet Package Files

When you install the `Deepstaging` NuGet package, two MSBuild files are automatically imported into your project:

**`build/Deepstaging.props`** (runs early):

- Defines `<DeepstagingDataDirectory>` (defaults to `.config`)
- Sets `<CompilerGeneratedFilesOutputPath>` to `!generated`
- Exposes `DeepstagingDataDirectory` as a `<CompilerVisibleProperty>` so analyzers and generators can read it
- Conditionally imports `$(DeepstagingDataDirectory)/deepstaging.props` from your project

**`build/Deepstaging.targets`** (runs late):

- Adds `$(DeepstagingDataDirectory)/**/*.schema.json` as `AdditionalFiles` so analyzers can detect and validate schemas
- Excludes `!generated/**` from compilation and includes them as `None` items
- Conditionally imports `$(DeepstagingDataDirectory)/deepstaging.targets` from your project

### Layer 2: Local Project Files (Code-Fix Generated)

When you apply the DSCFG06 code fix ("Generate configuration files"), Deepstaging creates project-local files inside your data directory:

**`{DataDirectory}/deepstaging.props`**:

- Per-project settings like `<UserSecretsId>` (when `[Secret]` properties exist)

**`{DataDirectory}/deepstaging.targets`**:

- `DependentUpon` metadata for IDE file nesting (schemas and settings nest under the props file)

These files are imported by Layer 1 via the conditional `<Import>` statements.

### Layer 3: Data Files

Also created by the code fix, these are the actual configuration artifacts:

- `deepstaging.schema.json` — JSON Schema for configuration validation
- `deepstaging.settings.json` — Settings template (+ per-environment variants)
- `user-secrets.schema.json` — Secrets schema (if `[Secret]` properties exist)
- `user-secrets.json` — Secrets file (gitignored)

## `DeepstagingDataDirectory`

The `DeepstagingDataDirectory` MSBuild property controls where all Deepstaging configuration files live. It defaults to `.config`.

### Overriding

Set it in your `.csproj` or `Directory.Build.props`:

```xml
<PropertyGroup>
    <DeepstagingDataDirectory>deepstaging</DeepstagingDataDirectory>
</PropertyGroup>
```

This changes the location of all generated configuration files, the import paths for local props/targets, and the schema glob.

### How It Flows

```
.csproj / Directory.Build.props
  └─ <DeepstagingDataDirectory>
       │
       ├─ NuGet Deepstaging.props reads it for <Import> paths
       ├─ NuGet Deepstaging.targets reads it for <AdditionalFiles> glob
       │
       ├─ <CompilerVisibleProperty> exposes it to Roslyn
       │    └─ Analyzers read it from GlobalOptions
       │         └─ Forwarded to code fixes via Diagnostic.Properties
       │              └─ Code fixes use it for file write paths
       │
       └─ Result: all files live under $(DeepstagingDataDirectory)/
```

## Default File Layout

With the default `.config` data directory, a project using `[ConfigProvider]` with `[Secret]` properties will have:

```
MyProject/
├── MyProject.csproj
├── .config/
│   ├── deepstaging.props
│   ├── deepstaging.targets
│   ├── deepstaging.schema.json
│   ├── deepstaging.settings.json
│   ├── deepstaging.settings.Development.json
│   ├── deepstaging.settings.Staging.json
│   ├── deepstaging.settings.Production.json
│   ├── user-secrets.schema.json
│   ├── user-secrets.json
│   └── .gitignore
└── !generated/
    └── (compiler-generated source files)
```

## Build Properties as a Communication Channel

`TrackedFileTypeAnalyzer` (from [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)) automatically forwards all MSBuild properties prefixed with `Deepstaging` into `Diagnostic.Properties`. This means:

1. Declare a `<CompilerVisibleProperty Include="DeepstagingMyProperty"/>` in your NuGet `.props`
2. The analyzer can read it from `AnalyzerConfigOptionsProvider.GlobalOptions`
3. It's automatically included in every diagnostic the analyzer reports
4. Code fixes read it from `diagnostic.Properties["DeepstagingMyProperty"]`

No per-property wiring needed — the base class handles discovery and forwarding.
