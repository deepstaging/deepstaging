# Deepstaging.Ids

Source-generated strongly-typed IDs for C#/.NET.

## Overview

This package provides the `[StrongId]` attribute that marks a partial struct as a strongly-typed ID. The source
generator produces equality, conversion operators, parsing, and optional serialization converters — eliminating
primitive obsession with zero boilerplate.

## Attribution

This project is a **clean-room implementation** inspired by Andrew
Lock's [StronglyTypedId](https://github.com/andrewlock/StronglyTypedId). The API surface and code generation approach
were designed independently, but the core idea of source-generated strongly-typed IDs originates from that project.

## Usage

```csharp
using Deepstaging.Ids;

// Default: Guid backing type, no converters
[StrongId]
public readonly partial struct UserId;

// Int backing type
[StrongId(BackingType = BackingType.Int)]
public readonly partial struct OrderId;

// With serialization converters
[StrongId(Converters = IdConverters.JsonConverter | IdConverters.EfCoreValueConverter)]
public readonly partial struct CustomerId;

// All converters
[StrongId(BackingType = BackingType.String, Converters = IdConverters.All)]
public readonly partial struct TenantId;
```

## Attributes

### [StrongId]

| Property      | Type           | Default | Description                               |
|---------------|----------------|---------|-------------------------------------------|
| `BackingType` | `BackingType`  | `Guid`  | Primitive type used to store the ID value |
| `Converters`  | `IdConverters` | `None`  | Which type converters to generate         |

### BackingType

| Value    | Backing Type            |
|----------|-------------------------|
| `Guid`   | `System.Guid` (default) |
| `Int`    | `int`                   |
| `Long`   | `long`                  |
| `String` | `string`                |

### IdConverters

| Value                  | Description                            |
|------------------------|----------------------------------------|
| `None`                 | No converters (default)                |
| `JsonConverter`        | System.Text.Json `JsonConverter`       |
| `EfCoreValueConverter` | Entity Framework Core `ValueConverter` |
| `TypeConverter`        | `System.ComponentModel.TypeConverter`  |
| `Dapper`               | Dapper `SqlMapper.TypeHandler`         |
| `NewtonsoftJson`       | Newtonsoft.Json `JsonConverter`        |
| `All`                  | All available converters               |

## Analyzers

| ID       | Severity | Description                        |
|----------|----------|------------------------------------|
| `ID0001` | Error    | StrongId struct must be partial    |
| `ID0002` | Warning  | StrongId struct should be readonly |

## Installation

```bash
dotnet add package Deepstaging.Ids
```

## Related Documentation

- **[Main README](../../README.md)** — Project overview and quick start
- **[Generators](../Deepstaging.Ids.Generators/README.md)** — How code is generated
- **[Analyzers](../Deepstaging.Ids.Analyzers/README.md)** — Compile-time validation
- **[Deepstaging.Roslyn](https://github.com/deepstaging/roslyn)** — Roslyn toolkit

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
