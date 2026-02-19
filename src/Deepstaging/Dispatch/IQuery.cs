// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Dispatch;

/// <summary>
/// Marker interface for query types that return a result of type <typeparamref name="TResult"/>.
/// Queries represent intent to read state without side effects.
/// </summary>
/// <typeparam name="TResult">The type of the query result.</typeparam>
public interface IQuery<TResult>;
