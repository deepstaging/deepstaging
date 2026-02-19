// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Dispatch;

/// <summary>
/// Interface for capabilities that support automatic commit after command dispatch.
/// </summary>
/// <remarks>
/// <para>
/// When a <see cref="DispatchModuleAttribute"/> has <c>AutoCommit = true</c> (default),
/// the generator wraps command dispatch with a commit call to all <see cref="IAutoCommittable"/>
/// capabilities referenced by the Runtime.
/// </para>
/// <para>
/// Common implementations include EF Core <c>DbContext.SaveChangesAsync()</c>, but any
/// capability can participate (e.g., cache flush, saga commit).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // EF Core DbContext implementing IAutoCommittable
/// public class AppDbContext : DbContext, IAutoCommittable
/// {
///     public Task CommitAsync(CancellationToken cancellationToken) =&gt;
///         SaveChangesAsync(cancellationToken);
/// }
/// </code>
/// </example>
public interface IAutoCommittable
{
    /// <summary>
    /// Commits any pending changes.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous commit operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}
