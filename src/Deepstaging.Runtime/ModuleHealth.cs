// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging;

/// <summary>
/// Represents the health status of a module.
/// </summary>
public enum ModuleHealth
{
    /// <summary>The module is operating normally.</summary>
    Healthy,

    /// <summary>The module is operational but experiencing issues.</summary>
    Degraded,

    /// <summary>The module is not operational.</summary>
    Unhealthy
}
