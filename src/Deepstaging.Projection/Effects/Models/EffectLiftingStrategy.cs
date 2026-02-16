// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Effects.Models;

/// <summary>
/// Defines how to lift a method's return type into Eff&lt;RT, A&gt;.
/// </summary>
public enum EffectLiftingStrategy
{
    /// <summary>T → Eff&lt;RT, T&gt;</summary>
    SyncValue,

    /// <summary>T? → Eff&lt;RT, Option&lt;T&gt;&gt;</summary>
    SyncNullableToOption,

    /// <summary>void → Eff&lt;RT, Unit&gt;</summary>
    SyncVoid,

    /// <summary>Task&lt;T&gt; → Eff&lt;RT, T&gt;</summary>
    AsyncValue,

    /// <summary>Task&lt;T?&gt; → Eff&lt;RT, Option&lt;T&gt;&gt;</summary>
    AsyncNullableToOption,

    /// <summary>Task → Eff&lt;RT, Unit&gt;</summary>
    AsyncVoid
}