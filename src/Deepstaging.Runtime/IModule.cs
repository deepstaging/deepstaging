// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging;

/// <summary>
/// Base contract for all Deepstaging modules. Provides identity and health reporting.
/// </summary>
/// <remarks>
/// All generated modules (effects, event queues, dispatch, etc.) implement this interface.
/// Consumers can enumerate <see cref="IModule"/> instances from the service provider
/// to build health dashboards, diagnostics endpoints, or startup validation.
/// </remarks>
public interface IModule
{
    /// <summary>
    /// The display name of the module.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The current health status of the module.
    /// </summary>
    ModuleHealth Health { get; }
}
