# Build Configuration

This directory contains modular MSBuild configuration files that are imported by `Directory.Build.props`.

## Files

### Build.Paths.props
Controls build output locations:
- `ArtifactsDir` - Root directory for all build outputs
- `BaseOutputPath` - Compiled binaries (.dll, .exe)
- `BaseIntermediateOutputPath` - Temporary build files (.obj)
- `PackageOutputPath` - NuGet packages (.nupkg)

**When to edit:** Changing where build outputs go

### Build.Language.props
C# language and code quality settings:
- `LangVersion` - C# language version
- `Nullable` - Nullable reference types
- `ImplicitUsings` - Auto-import common namespaces
- `TreatWarningsAsErrors` - Build fails on warnings

**When to edit:** Adjusting language features or strictness

### Build.Packaging.props
NuGet package metadata and versioning:
- Package metadata (Authors, Company, Description, etc.)
- License and repository URLs
- Package tags and assets (README, icon)
- Version prefix

**When to edit:** Customizing package information before publishing

### Build.SourceLink.props
Debugging and documentation:
- XML documentation generation
- SourceLink integration (source code debugging)
- Symbol package configuration (.snupkg)

**When to edit:** Rarely - defaults work for most projects

## Import Order

Files are imported in this order by `Directory.Build.props`:
1. Build.Paths.props
2. Build.Language.props
3. Build.Packaging.props
4. Build.SourceLink.props
5. Directory.Build.Dev.props (if exists, Debug only)

## Adding New Configuration

To add a new category:
1. Create `Build.YourCategory.props`
2. Add `<Import Project="$(MSBuildThisFileDirectory)build/Build.YourCategory.props" />`
3. Document it in this README

## Learn More

- [Customize your build](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory)
- [Common MSBuild properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties)
