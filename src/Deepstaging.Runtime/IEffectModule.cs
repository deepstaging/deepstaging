// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging;

/// <summary>
/// Marker interface for modules that can be composed into a Runtime via <c>[Uses]</c>.
/// </summary>
/// <remarks>
/// <para>
/// When a type implementing <see cref="IEffectModule"/> is referenced by
/// <c>[Uses(typeof(MyModule))]</c> on a Runtime class, the Effects generator
/// recognizes it and generates capability interfaces and Eff accessor methods.
/// </para>
/// <para>
/// Built-in module types (<c>[EffectsModule]</c>, <c>[EventQueue]</c>, <c>[DispatchModule]</c>)
/// generate implementations of this interface automatically. Third-party modules can also
/// implement this interface to participate in Runtime composition.
/// </para>
/// </remarks>
public interface IEffectModule : IModule;
