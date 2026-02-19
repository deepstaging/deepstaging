# Typed IDs Module

The Typed IDs module generates type-safe ID structs with configurable backing types and serialization converters.

## Overview

```csharp
using Deepstaging.Ids;

[TypedId]
public readonly partial struct UserId;

[TypedId(BackingType = BackingType.Int, Converters = IdConverters.JsonConverter | IdConverters.EfCoreValueConverter)]
public readonly partial struct OrderId;
```

Generated structs implement:

- `IEquatable<T>` — value equality
- `IParsable<T>` — string parsing
- `IComparable<T>` — sorting
- `ToString()` — formatted output
- Factory method (`New()`) — create new instances

## Attributes

### `[TypedId]`

Marks a partial struct as a strongly-typed ID.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BackingType` | `BackingType` | `Guid` | The underlying value type |
| `Converters` | `IdConverters` | `None` | Which serialization converters to generate |

### Backing Types

| Value | CLR Type | Factory |
|-------|----------|---------|
| `Guid` | `System.Guid` | `New()` generates a random GUID |
| `Int` | `int` | `New(int value)` wraps the value |
| `Long` | `long` | `New(long value)` wraps the value |
| `String` | `string` | `New(string value)` wraps the value |

### Converters

`IdConverters` is a flags enum — combine with `|`:

| Flag | What it generates |
|------|-------------------|
| `JsonConverter` | `System.Text.Json` converter |
| `EfCoreValueConverter` | EF Core `ValueConverter<T, TBacking>` |

A `System.ComponentModel.TypeConverter` is always generated regardless of flags.

```csharp
[TypedId(Converters = IdConverters.JsonConverter | IdConverters.EfCoreValueConverter)]
public readonly partial struct ProductId;
```

## Diagnostics

| ID | Severity | Description |
|----|----------|-------------|
| DSID01 | Error | TypedId struct must be partial |
| DSID02 | Warning | TypedId struct should be readonly |
