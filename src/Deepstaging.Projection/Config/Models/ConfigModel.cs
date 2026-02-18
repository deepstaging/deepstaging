// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Config.Models;

using System.Linq;

/// <summary>
/// Represents a configuration provider model built from a type decorated with <see cref="ConfigProviderAttribute"/>.
/// </summary>
[PipelineModel]
public sealed record ConfigModel
{
    /// <summary>
    /// The namespace containing the type.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The simple name of the type.
    /// </summary>
    public required TypeRef TypeName { get; init; }

    /// <summary>
    /// The accessibility modifier of the type.
    /// </summary>
    public required string Accessibility { get; init; }

    /// <summary>
    /// The configuration section name (explicit or inferred from the class name).
    /// </summary>
    public required string Section { get; init; }

    /// <summary>
    /// The configuration types exposed by this provider.
    /// </summary>
    public EquatableArray<ConfigTypeModel> ExposedConfigurationTypes { get; init; } = [];

    /// <summary>
    /// Whether any exposed configuration type contains properties marked with <c>[Secret]</c>.
    /// </summary>
    public bool HasSecrets => ExposedConfigurationTypes.Any(ct => ct.Properties.Any(p => p.IsSecret));

    /// <summary>
    /// 
    /// </summary>
    public string DataDirectory { get; init; } = ".config";
}