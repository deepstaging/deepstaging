// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Projection.Config.Models;

/// <summary>
/// Represents a configuration type that should be exposed as a property on the generated config class.
/// </summary>
/// <param name="Type">The type of the configuration property.</param>
/// <param name="Properties">The properties of the configuration type that should be exposed.</param>
[PipelineModel]
public sealed record ConfigTypeModel(TypeSnapshot Type, EquatableArray<ConfigTypePropertyModel> Properties);