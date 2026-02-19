---
applyTo: src/**/Ids/**,src/**/*TypedId*
---

# Ids Module

Namespace: `Deepstaging.Ids`

Generates strongly-typed ID structs with configurable backing types and serialization converters.

## Attributes

- `TypedIdAttribute` — `BackingType` (Guid/Int/Long/String, default: Guid), `Converters` (flags: JsonConverter, EfCoreValueConverter). A `System.ComponentModel.TypeConverter` is always generated.

## Diagnostic IDs

| ID | Severity | Description |
|----|----------|-------------|
| DSID01 | Error | TypedId struct must be partial |
| DSID02 | Warning | TypedId struct should be readonly |

## Projection Models

- `TypedIdModel` — namespace, typeName, accessibility, backingType, converters, backingTypeSnapshot

## Writers

All in `Deepstaging.Generators/Writers/Ids/`:

- `TypedIdWriter` — main struct implementation (IEquatable, IParsable, ToString, etc.)
- Partials: `.Constructor`, `.Factory`, `.TypeConverter`, `.JsonConverter`, `.EfCoreValueConverter`, `.Converters`
