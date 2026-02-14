// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Linq.Expressions;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;

namespace Deepstaging.Effects.Runtime;

/// <summary>
/// A composable query builder for DbSet that accumulates LINQ expressions
    /// and materializes to an Eff only on terminal operations.
/// </summary>
/// <typeparam name="RT">The runtime type.</typeparam>
/// <typeparam name="T">The entity type.</typeparam>
public sealed class DbSetQuery<RT, T> where T : class
{
    private readonly Func<RT, IQueryable<T>> _queryFactory;

    /// <summary>
    /// Creates a new query builder from a factory function.
    /// </summary>
    public DbSetQuery(Func<RT, IQueryable<T>> queryFactory)
    {
        _queryFactory = queryFactory;
    }

    // ========== Filtering ==========

    /// <summary>
    /// Filters the sequence based on a predicate.
    /// </summary>
    public DbSetQuery<RT, T> Where(Expression<Func<T, bool>> predicate)
    {
        return new DbSetQuery<RT, T>(rt => _queryFactory(rt).Where(predicate));
    }

    // ========== Ordering ==========

    /// <summary>
    /// Sorts the elements in ascending order.
    /// </summary>
    public DbSetOrderedQuery<RT, T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        return new DbSetOrderedQuery<RT, T>(rt => _queryFactory(rt).OrderBy(keySelector));
    }

    /// <summary>
    /// Sorts the elements in descending order.
    /// </summary>
    public DbSetOrderedQuery<RT, T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        return new DbSetOrderedQuery<RT, T>(rt => _queryFactory(rt).OrderByDescending(keySelector));
    }

    // ========== Eager Loading ==========

    /// <summary>
    /// Includes a related entity in the query results.
    /// </summary>
    public DbSetQuery<RT, T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
    {
        return new DbSetQuery<RT, T>(rt => _queryFactory(rt).Include(navigationPropertyPath));
    }

    /// <summary>
    /// Includes a related entity using a string path.
    /// </summary>
    public DbSetQuery<RT, T> Include(string navigationPropertyPath)
    {
        return new DbSetQuery<RT, T>(rt => _queryFactory(rt).Include(navigationPropertyPath));
    }

    // ========== Pagination ==========

    /// <summary>
    /// Bypasses a specified number of elements.
    /// </summary>
    public DbSetQuery<RT, T> Skip(int count)
    {
        return new DbSetQuery<RT, T>(rt => _queryFactory(rt).Skip(count));
    }

    /// <summary>
    /// Returns a specified number of elements.
    /// </summary>
    public DbSetQuery<RT, T> Take(int count)
    {
        return new DbSetQuery<RT, T>(rt => _queryFactory(rt).Take(count));
    }

    // ========== Projection ==========

    /// <summary>
    /// Projects each element to a new form.
    /// </summary>
    public DbSetQuery<RT, TResult> Select<TResult>(Expression<Func<T, TResult>> selector) where TResult : class
    {
        return new DbSetQuery<RT, TResult>(rt => _queryFactory(rt).Select(selector));
    }

    /// <summary>
    /// Projects and flattens sequences.
    /// </summary>
    public DbSetQuery<RT, TResult> SelectMany<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector)
        where TResult : class
    {
        return new DbSetQuery<RT, TResult>(rt => _queryFactory(rt).SelectMany(selector));
    }

    // ========== Set Operations ==========

    /// <summary>
    /// Returns distinct elements.
    /// </summary>
    public DbSetQuery<RT, T> Distinct()
    {
        return new DbSetQuery<RT, T>(rt => Queryable.Distinct(_queryFactory(rt)));
    }

    // ========== Tracking ==========

    /// <summary>
    /// Disables change tracking for the query.
    /// </summary>
    public DbSetQuery<RT, T> AsNoTracking()
    {
        return new DbSetQuery<RT, T>(rt => _queryFactory(rt).AsNoTracking());
    }

    /// <summary>
    /// Enables change tracking with identity resolution.
    /// </summary>
    public DbSetQuery<RT, T> AsNoTrackingWithIdentityResolution()
    {
        return new DbSetQuery<RT, T>(rt => _queryFactory(rt).AsNoTrackingWithIdentityResolution());
    }

    /// <summary>
    /// Enables change tracking.
    /// </summary>
    public DbSetQuery<RT, T> AsTracking()
    {
        return new DbSetQuery<RT, T>(rt => _queryFactory(rt).AsTracking());
    }

    // ========== Terminal Operations (return Eff) ==========

    /// <summary>
    /// Executes the query and returns all results as a list.
    /// </summary>
    public Eff<RT, List<T>> ToListAsync(CancellationToken token = default)
    {
        return Eff<RT, List<T>>.LiftIO(async rt => await _queryFactory(rt).ToListAsync(token));
    }

    /// <summary>
    /// Executes the query and returns all results as an array.
    /// </summary>
    public Eff<RT, T[]> ToArrayAsync(CancellationToken token = default)
    {
        return Eff<RT, T[]>.LiftIO(async rt => await _queryFactory(rt).ToArrayAsync(token));
    }

    /// <summary>
    /// Returns the first element, or None if empty.
    /// </summary>
    public Eff<RT, Option<T>> FirstOrNoneAsync(CancellationToken token = default)
    {
        return Eff<RT, Option<T>>.LiftIO(async rt => Optional(await _queryFactory(rt).FirstOrDefaultAsync(token)));
    }

    /// <summary>
    /// Returns the first element matching the predicate, or None if not found.
    /// </summary>
    public Eff<RT, Option<T>> FirstOrNoneAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return Eff<RT, Option<T>>.LiftIO(async rt =>
            Optional(await _queryFactory(rt).FirstOrDefaultAsync(predicate, token)));
    }

    /// <summary>
    /// Returns the single element, or None if empty. Throws if more than one.
    /// </summary>
    public Eff<RT, Option<T>> SingleOrNoneAsync(CancellationToken token = default)
    {
        return Eff<RT, Option<T>>.LiftIO(async rt => Optional(await _queryFactory(rt).SingleOrDefaultAsync(token)));
    }

    /// <summary>
    /// Returns the single element matching the predicate, or None. Throws if more than one.
    /// </summary>
    public Eff<RT, Option<T>> SingleOrNoneAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return Eff<RT, Option<T>>.LiftIO(async rt =>
            Optional(await _queryFactory(rt).SingleOrDefaultAsync(predicate, token)));
    }

    /// <summary>
    /// Returns the count of elements.
    /// </summary>
    public Eff<RT, int> CountAsync(CancellationToken token = default)
    {
        return Eff<RT, int>.LiftIO(async rt => await _queryFactory(rt).CountAsync(token));
    }

    /// <summary>
    /// Returns the count of elements matching the predicate.
    /// </summary>
    public Eff<RT, int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return Eff<RT, int>.LiftIO(async rt => await _queryFactory(rt).CountAsync(predicate, token));
    }

    /// <summary>
    /// Returns the long count of elements.
    /// </summary>
    public Eff<RT, long> LongCountAsync(CancellationToken token = default)
    {
        return Eff<RT, long>.LiftIO(async rt => await _queryFactory(rt).LongCountAsync(token));
    }

    /// <summary>
    /// Returns true if any elements exist.
    /// </summary>
    public Eff<RT, bool> AnyAsync(CancellationToken token = default)
    {
        return Eff<RT, bool>.LiftIO(async rt => await _queryFactory(rt).AnyAsync(token));
    }

    /// <summary>
    /// Returns true if any elements match the predicate.
    /// </summary>
    public Eff<RT, bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return Eff<RT, bool>.LiftIO(async rt => await _queryFactory(rt).AnyAsync(predicate, token));
    }

    /// <summary>
    /// Returns true if all elements match the predicate.
    /// </summary>
    public Eff<RT, bool> AllAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return Eff<RT, bool>.LiftIO(async rt => await _queryFactory(rt).AllAsync(predicate, token));
    }

    // ========== Aggregates ==========

    /// <summary>
    /// Returns the maximum value.
    /// </summary>
    public Eff<RT, T> MaxAsync(CancellationToken token = default)
    {
        return Eff<RT, T>.LiftIO(async rt => (await _queryFactory(rt).MaxAsync(token))!);
    }

    /// <summary>
    /// Returns the maximum value of the selected property.
    /// </summary>
    public Eff<RT, TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken token = default)
    {
        return Eff<RT, TResult>.LiftIO(async rt => (await _queryFactory(rt).MaxAsync(selector, token))!);
    }

    /// <summary>
    /// Returns the minimum value.
    /// </summary>
    public Eff<RT, T> MinAsync(CancellationToken token = default)
    {
        return Eff<RT, T>.LiftIO(async rt => (await _queryFactory(rt).MinAsync(token))!);
    }

    /// <summary>
    /// Returns the minimum value of the selected property.
    /// </summary>
    public Eff<RT, TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken token = default)
    {
        return Eff<RT, TResult>.LiftIO(async rt => (await _queryFactory(rt).MinAsync(selector, token))!);
    }
}

/// <summary>
/// An ordered query builder that supports ThenBy operations.
/// </summary>
public sealed class DbSetOrderedQuery<RT, T> where T : class
{
    private readonly Func<RT, IOrderedQueryable<T>> _queryFactory;

    internal DbSetOrderedQuery(Func<RT, IOrderedQueryable<T>> queryFactory)
    {
        _queryFactory = queryFactory;
    }

    /// <summary>
    /// Performs a subsequent ordering in ascending order.
    /// </summary>
    public DbSetOrderedQuery<RT, T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        return new DbSetOrderedQuery<RT, T>(rt => _queryFactory(rt).ThenBy(keySelector));
    }

    /// <summary>
    /// Performs a subsequent ordering in descending order.
    /// </summary>
    public DbSetOrderedQuery<RT, T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        return new DbSetOrderedQuery<RT, T>(rt => _queryFactory(rt).ThenByDescending(keySelector));
    }

    // ========== Pagination (after ordering) ==========

    /// <inheritdoc cref="DbSetQuery{RT,T}.Skip"/>
    public DbSetQuery<RT, T> Skip(int count)
    {
        return new DbSetQuery<RT, T>(rt => _queryFactory(rt).Skip(count));
    }

    /// <inheritdoc cref="DbSetQuery{RT,T}.Take"/>
    public DbSetQuery<RT, T> Take(int count)
    {
        return new DbSetQuery<RT, T>(rt => _queryFactory(rt).Take(count));
    }

    // ========== Terminal Operations ==========

    /// <inheritdoc cref="DbSetQuery{RT,T}.ToListAsync"/>
    public Eff<RT, List<T>> ToListAsync(CancellationToken token = default)
    {
        return Eff<RT, List<T>>.LiftIO(async rt => await _queryFactory(rt).ToListAsync(token));
    }

    /// <inheritdoc cref="DbSetQuery{RT,T}.ToArrayAsync"/>
    public Eff<RT, T[]> ToArrayAsync(CancellationToken token = default)
    {
        return Eff<RT, T[]>.LiftIO(async rt => await _queryFactory(rt).ToArrayAsync(token));
    }

    /// <inheritdoc cref="DbSetQuery{RT,T}.FirstOrNoneAsync(CancellationToken)"/>
    public Eff<RT, Option<T>> FirstOrNoneAsync(CancellationToken token = default)
    {
        return Eff<RT, Option<T>>.LiftIO(async rt => Optional(await _queryFactory(rt).FirstOrDefaultAsync(token)));
    }
}