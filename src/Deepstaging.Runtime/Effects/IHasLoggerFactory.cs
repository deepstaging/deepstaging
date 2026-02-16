// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Effects;

/// <summary>
/// Runtime capability interface for providing an <see cref="ILoggerFactory"/> to effect instrumentation.
/// </summary>
/// <remarks>
/// When the runtime implements this interface, <see cref="ActivityEffectExtensions.WithActivity{RT,A}"/>
/// automatically resolves a logger for structured logging of effect operations.
/// </remarks>
public interface IHasLoggerFactory
{
    /// <summary>
    /// The logger factory used to create loggers for effect instrumentation.
    /// </summary>
    ILoggerFactory LoggerFactory { get; }
}
