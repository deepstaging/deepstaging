// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Projection.Config.Models;

/// <summary>
/// Represents a property of a configuration type that should be exposed on the generated config class.
/// </summary>
/// <param name="Property">The property to expose.</param>
/// <param name="Documentation">The XML documentation for the property (if available).</param>
/// <param name="IsSecret">Whether this property should be treated as a secret (e.g., stored in user-secrets).</param>
[PipelineModel]
public sealed record ConfigTypePropertyModel(PropertySnapshot Property, DocumentationSnapshot Documentation, bool IsSecret);