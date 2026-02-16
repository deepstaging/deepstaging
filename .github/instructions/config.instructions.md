---
applyTo: src/**/Config/**,src/**/*Config*
---

# Config Module

Namespace: `Deepstaging.Config`

Generates strongly-typed configuration classes with exposed configuration types.

## Attributes

- `ConfigRootAttribute` — marks a class/struct as a configuration root (no properties)
- `ExposesAttribute<T>` — declares a configuration type to expose from the root (no properties)

## Diagnostic IDs

None currently.

## Projection Models

- `ConfigModel` — namespace, typeName, accessibility, exposedConfigurationTypes
- `ConfigTypeModel` — type snapshot, properties
- `ConfigTypePropertyModel` — property snapshot, documentation, isSecret

## Writers

In `Deepstaging.Generators/Writers/Config/`:

- `ConfigWriter` — generates partial config class and interface
