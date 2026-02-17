# Copilot Instructions for Deepstaging

## Build, Test, Lint

```bash
# Build
dotnet build Deepstaging.slnx

# Test (all) — prefer `dotnet run` for TUnit (easier flag passing)
dotnet run --project test/Deepstaging.Tests -c Release
dotnet run --project test/Deepstaging.Testing.Tests -c Release

# Test (by class name)
dotnet run --project test/Deepstaging.Tests -c Release --treenode-filter /*/*/StrongIdGeneratorTests/*

# Test (by test name)
dotnet run --project test/Deepstaging.Tests -c Release --treenode-filter /*/*/*/GeneratesGuidId_WithDefaultSettings

# Test (by feature — e.g., all Effects tests)
dotnet run --project test/Deepstaging.Tests -c Release --treenode-filter /*/Deepstaging.Tests*Effects*/*/*

# Test via dotnet test (flags go after --)
dotnet test --project test/Deepstaging.Tests -c Release -- --treenode-filter /*/*/StrongIdGeneratorTests/*

# Pack (local dev)
./build/pack.sh
```

There is no separate lint command. Warnings are treated as errors via `TreatWarningsAsErrors`, so `dotnet build` is the lint step.

The `--treenode-filter` syntax is `/<Assembly>/<Namespace>/<Class>/<Test>` with `*` wildcards.

## Architecture

### Project Map

| Project | Target | Purpose |
|---------|--------|---------|
| `Deepstaging` | netstandard2.0 + net10.0 | Core marker attributes (`EffectsModuleAttribute`, `StrongIdAttribute`, `ConfigRootAttribute`, `HttpClientAttribute`) and enums (`BackingType`, `IdConverters`) |
| `Deepstaging.Runtime` | net10.0 | Runtime support: OpenTelemetry instrumentation (`ActivityEffectExtensions`), `IHasLoggerFactory`, `EffectMetrics` |
| `Deepstaging.Projection` | netstandard2.0 | Roslyn analysis layer — attribute queries and pipeline models for each feature |
| `Deepstaging.Generators` | netstandard2.0 | Incremental source generators: `EffectsGenerator`, `StrongIdGenerator`, `ConfigGenerator`, `HttpClientGenerator` |
| `Deepstaging.Analyzers` | netstandard2.0 | Roslyn diagnostic analyzers (enforce partial/sealed, validate targets, check method names) |
| `Deepstaging.CodeFixes` | netstandard2.0 | Code fix providers (`StructMustBePartialCodeFix`, `ClassMustBePartialCodeFix`, etc.) |
| `Deepstaging.Testing` | netstandard2.0 + net10.0 | Test support library (not tests) — `ITestRuntime<TSelf>`, `TestRuntimeAttribute<TRuntime>` |
| `Deepstaging.Testing.Projection` | netstandard2.0 | Projection layer for test runtime attributes |
| `Deepstaging.Testing.Generators` | netstandard2.0 | Generator for test runtime implementations |
| `Deepstaging.Testing.Analyzers` | netstandard2.0 | Analyzers for test runtime attributes |
| `Deepstaging.Testing.CodeFixes` | netstandard2.0 | Code fixes for test runtime violations |
| `Deepstaging.Tests` | net10.0 | Main test suite (in `test/`) |
| `Deepstaging.Testing.Tests` | net10.0 | Tests for the Testing support library (in `test/`) |

All library projects target `netstandard2.0` for Roslyn analyzer/generator compatibility. Only test projects and Runtime target `net10.0`.

### Layered Architecture

Each feature (Effects, Ids, Config, HttpClient) follows this pipeline:

```
Attributes (Deepstaging)
    ↓
Projection (Deepstaging.Projection) ← single source of truth for attribute interpretation
    ↓
Generators, Analyzers, CodeFixes
```

### Generator Pattern

Generators use the Emit API with Writer classes organized by feature:

```
Deepstaging.Generators/
├── Effects/
│   ├── EffectsGenerator.cs              # IIncrementalGenerator entry point
│   └── Writers/
│       ├── EffectsModuleWriter.cs        # Core writer
│       ├── EffectsModuleWriter.Methods.cs # Partial for method generation
│       ├── EffectsModuleWriter.DbContext.cs
│       ├── DbSetQueryWriter.cs
│       ├── RuntimeWriter.cs
│       └── RuntimeBootstrapperWriter.cs
├── Ids/Writers/StrongIdWriter.cs         # + partials: .Constructor, .Converters, .Factory, etc.
├── Config/Writers/ConfigWriter.cs
└── HttpClient/Writers/
    ├── ClientWriter.cs
    ├── InterfaceWriter.cs
    └── RequestWriter.cs
```

Generator entry point pattern:
```csharp
[Generator]
public sealed class StrongIdGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.ForAttribute<StrongIdAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().ToStrongIdModel(ctx.SemanticModel));

        context.RegisterSourceOutput(models, static (ctx, model) =>
        {
            model.WriteStrongId()
                .AddSourceTo(ctx, HintName.From(model.Namespace, model.TypeName));
        });
    }
}
```

Writers transform projection models into generated code using TypeBuilder fluent API:
```csharp
model.WriteStrongId()        // returns OptionalEmit
    .AddSourceTo(ctx, hint);
```

### Analyzer Pattern

Analyzers extend `TypeAnalyzer` (from Deepstaging.Roslyn):
```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[Reports(DiagnosticId, "Title", Message = "...", Description = "...")]
public sealed class StrongIdMustBePartialAnalyzer : TypeAnalyzer
{
    public const string DiagnosticId = "ID0001";

    protected override bool ShouldReport(ValidSymbol<INamedTypeSymbol> type) =>
        type.HasAttribute<StrongIdAttribute>() && type is { IsPartial: false };
}
```

### CodeFix Pattern

Code fixes use helper base classes:
```csharp
[Shared]
[CodeFix(StrongIdMustBePartialAnalyzer.DiagnosticId)]
[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class StructMustBePartialCodeFix : StructCodeFix
{
    protected override CodeAction CreateFix(Document document, ValidSyntax<StructDeclarationSyntax> syntax) =>
        document.AddPartialModifierAction(syntax);
}
```

## Testing

### Test Framework

Uses **TUnit** (not xUnit/NUnit) with async assertions and **Verify** for snapshot testing.

All Roslyn tests inherit from `RoslynTestBase` (from `Deepstaging.Roslyn.Testing`).

### Test Patterns

**Generator tests:**
```csharp
await GenerateWith<StrongIdGenerator>(source)
    .ShouldGenerate()
    .WithFileContaining("public partial struct UserId")
    .WithNoDiagnostics()
    .VerifySnapshot();
```

**Analyzer tests:**
```csharp
await AnalyzeWith<StrongIdMustBePartialAnalyzer>(source)
    .ShouldReportDiagnostic("ID0001")
    .WithSeverity(DiagnosticSeverity.Error)
    .WithMessage("*UserId*partial*");
```

**CodeFix tests:**
```csharp
await AnalyzeAndFixWith<StrongIdMustBePartialAnalyzer, StructMustBePartialCodeFix>(source)
    .ForDiagnostic("ID0001")
    .ShouldProduce(expectedSource);
```

**Writer unit tests** (using `SymbolsFor` to test emit logic directly):
```csharp
var emit = SymbolsFor(source)
    .RequireNamedType("EmailEffects")
    .QueryEffectsModules()
    .First()
    .WriteCapabilityInterface();

await Assert.That(emit).IsSuccessful();
```

### Reference Configuration

Tests register assembly references via `ModuleInitializer` in `TestConfiguration.cs`:
```csharp
[ModuleInitializer]
public static void Initialize()
{
    ReferenceConfiguration.AddReferencesFromTypes(
        typeof(EffectsModuleAttribute),
        typeof(ActivityEffectExtensions),
        // ...all types needed for compilation
    );
}
```

### Snapshot Testing

Uses **Verify** for generator output. Snapshots stored alongside tests with `.verified.txt` extension.

### Test Directory Structure

Tests mirror the feature structure:
```
test/
├── Deepstaging.Tests/
│   ├── Generators/Effects/, Ids/, Config/, HttpClient/
│   ├── Analyzers/Effects/, Ids/, Config/, HttpClient/
│   ├── CodeFixes/Effects/, Ids/, Config/
│   └── Projection/Effects/, Ids/
└── Deepstaging.Testing.Tests/
```

## Conventions

### License Headers (Required)

Every source file must start with SPDX license headers. A pre-commit hook enforces this.

```csharp
// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
```

### Code Style

- C# `latest` language version (C# 14 features including extension members)
- Nullable reference types enabled everywhere
- `TreatWarningsAsErrors` — `dotnet build` is the lint step
- Central package versioning via `Directory.Packages.props` — never specify versions in `.csproj`
- Build config is modular: `build/Build.*.props` files — edit there, not in `Directory.Build.props`
- `Deepstaging.Versions.props` is auto-updated by CI — do not edit manually
- Local dev overrides go in `Directory.Build.Dev.props` (gitignored, template provided)

### Projection Pattern

Use `Optional*` and `Valid*` wrappers instead of null checks:
```csharp
// Early exit pattern
if (optional.IsNotValid(out var valid))
    return;
// valid is now guaranteed non-null
```

### C# Extensions

This codebase uses C# 14 extension members (not classic extension methods):
```csharp
extension(ValidSymbol<INamedTypeSymbol> symbol)
{
    public ImmutableArray<StrongIdAttributeQuery> StrongIdAttributes() =>
        [..symbol.GetAttributes<StrongIdAttribute>().Select(...)];
}
```

### NuGet Configuration

- Local source at `../../artifacts/packages` for Deepstaging.Roslyn packages (sibling repo)
- Source mapping: `Deepstaging.*` → local first, nuget.org fallback
