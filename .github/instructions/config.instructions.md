---
applyTo: src/**/Config/**,src/**/*Config*
---

# Config Module

Namespace: `Deepstaging.Config`

Generates strongly-typed configuration providers with interfaces, DI registration, and JSON Schema output.

## Attributes

- `ConfigProviderAttribute` — marks a partial class as a configuration provider. Optional `Section` property (inferred by stripping `ConfigProvider` suffix from class name if omitted).
- `ExposesAttribute<T>` — declares a configuration type to expose from the provider (repeatable)
- `SecretAttribute` — marks a property as sensitive; excluded from deepstaging schema, included in secrets schema

## Diagnostic IDs

| ID | Severity | Description |
|----|----------|-------------|
| CFG001 | Error | ConfigProvider class must be partial |
| CFG002 | Warning | ConfigProvider class should be sealed |
| CFG003 | Error | Section name could not be inferred (no `ConfigProvider` suffix and no explicit `Section`) |
| CFG004 | Warning | Exposed type has no public instance properties |
| CFG005 | Warning | Property appears to contain secrets/PII — consider adding `[Secret]` |
| CFG006 | Info/Warning | Configuration files missing (Info) or out of date (Warning) |
| CFG007 | Error | [Secret] properties exist but assembly has no UserSecretsId — run `dotnet user-secrets init` |

## Code Fixes

| Diagnostic | Fix | Class |
|------------|-----|-------|
| CFG001 | Add `partial` modifier | `ClassMustBePartialCodeFix` (shared) |
| CFG002 | Add `sealed` modifier | `ClassShouldBeSealedCodeFix` (shared) |
| CFG005 | Add `[Secret]` attribute | `AddSecretAttributeCodeFix` |
| CFG006 | Generate all configuration files | `GenerateConfigFilesCodeFix` |

## Projection Models

- `ConfigModel` — namespace, typeName, accessibility, section, exposedConfigurationTypes
- `ConfigTypeModel` — type snapshot, properties
- `ConfigTypePropertyModel` — property snapshot, documentation, isSecret
- `ConfigProviderAttributeQuery` — wraps `AttributeData`, provides `GetSectionName(ValidSymbol)` for section inference

## Schema

In `Deepstaging.Projection/Config/Schema/`:

- `JsonSchemaBuilder` — builds JSON Schema Draft-7 from `ConfigModel`. Methods: `BuildAppSettingsSchema()`, `BuildSecretsSchema()`, `MapToJsonSchemaType()`. Embeds a `$comment` with a SHA-256 hash for staleness tracking.
- `SchemaHash` — computes deterministic SHA-256 hash from `ConfigModel`; extracts hash from existing schema content
- `SchemaFiles` — reads `*.schema.json` from `AdditionalFiles`, provides file existence and hash lookup

## Writers

In `Deepstaging.Generators/Writers/Config/`:

- `ConfigWriter` — generates interface (`I{TypeName}`), partial class with `IConfiguration` binding, and DI registration extension class
