// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Patterns;

namespace Deepstaging.Ids.Generators.Writers;

/// <summary>
/// Core member generation for strongly-typed IDs.
/// </summary>
internal static class CoreWriter
{
    /// <summary>
    /// Adds the constructor that takes the backing value.
    /// String types require null-checking with different APIs per .NET version.
    /// </summary>
    internal static TypeBuilder AddConstructor(this TypeBuilder builder, StrongIdModel model)
    {
        var backingTypeName = model.BackingTypeSnapshot.FullyQualifiedName;

        if (model.BackingType != BackingType.String)
            return builder
                .AddConstructor(c => c
                    .AddParameter("value", backingTypeName)
                    .WithBody(b => b.AddStatement("Value = value;")));

        return builder
            .AddConstructor(c => c
                .When(Directives.Net7OrGreater)
                .AddParameter("value", backingTypeName, b => b.ThrowIfNullOrEmpty())
                .WithBody(b => b
                    .AddStatement("Value = value;")))
            .AddConstructor(c => c
                .When(Directives.NotNet7OrGreater)
                .AddParameter("value", backingTypeName)
                .WithBody(b => b
                    .AddStatement(
                        "Value = value ?? throw new global::System.ArgumentNullException(nameof(value));")));
    }
}
