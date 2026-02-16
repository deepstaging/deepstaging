---
applyTo: src/**/Ids/**,src/**/*StrongId*
---

# Ids Module

Namespace: `Deepstaging.Ids`

Generates strongly-typed ID structs with configurable backing types and serialization converters.

## Attributes

- `StrongIdAttribute` — `BackingType` (Guid/Int/Long/String, default: Guid), `Converters` (flags: JsonConverter, EfCoreValueConverter, DapperTypeHandler, NewtonsoftJsonConverter, All)

## Diagnostic IDs

| ID | Severity | Description |
|----|----------|-------------|
| ID0001 | Error | StrongId struct must be partial |
| ID0002 | Warning | StrongId struct should be readonly |

## Projection Models

- `StrongIdModel` — namespace, typeName, accessibility, backingType, converters, backingTypeSnapshot

## Writers

All in `Deepstaging.Generators/Writers/Ids/`:

- `StrongIdWriter` — main struct implementation (IEquatable, IParsable, ToString, etc.)
- Partials: `.Constructor`, `.Factory`, `.TypeConverter`, `.JsonConverter`, `.NewtonsoftJsonConverter`, `.EfCoreValueConverter`, `.DapperTypeHandler`, `.Converters`
