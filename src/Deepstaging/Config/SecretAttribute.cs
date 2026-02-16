// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Config;

/// <summary>
/// Marks a configuration property as containing sensitive data (e.g. passwords, API keys, tokens).
/// Properties with this attribute are placed in the secrets schema rather than appsettings.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class SecretAttribute : Attribute;
