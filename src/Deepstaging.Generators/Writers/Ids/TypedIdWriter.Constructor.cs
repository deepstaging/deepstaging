// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.Ids;

using Projection.Ids.Models;

/// <summary>
/// Core member generation for strongly-typed IDs.
/// </summary>
internal static class ConstructorWriter
{
    /// <summary>
    /// Adds the constructor that takes the backing value.
    /// String types require null-checking with different APIs per .NET version.
    /// </summary>
    internal static TypeBuilder AddConstructor(this TypeBuilder builder, TypedIdModel model)
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
                    .AddStatement($"Value = value ?? throw {ExceptionRefs.ArgumentNull.New("nameof(value)")}")));
    }
}