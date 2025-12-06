using System;
using System.Linq;
using System.Linq.Expressions;

namespace ICG.NetCore.Utilities;

/// <summary>
/// Provides extension methods for <see cref="IQueryable{T}"/> to simplify conditional filtering, ordering, paging, and distinct selection.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Conditionally applies a filter to the query if the specified condition is true.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <param name="source">The source queryable sequence.</param>
    /// <param name="condition">If true, the predicate is applied; otherwise, the source is returned unchanged.</param>
    /// <param name="predicate">The filter expression to apply.</param>
    /// <returns>The filtered queryable sequence if <paramref name="condition"/> is true; otherwise, the original sequence.</returns>
    public static IQueryable<TSource> WhereIf<TSource>(
        this IQueryable<TSource> source,
        bool condition,
        Expression<Func<TSource, bool>> predicate)
    {
        if (condition)
        {
            return source.Where(predicate);
        }
        return source;
    }

    /// <summary>
    /// Conditionally applies an ascending order to the query if the specified condition is true.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key to order by.</typeparam>
    /// <param name="query">The source queryable sequence.</param>
    /// <param name="condition">If true, the ordering is applied; otherwise, the source is returned unchanged.</param>
    /// <param name="orderBy">The key selector expression for ordering.</param>
    /// <returns>The ordered queryable sequence if <paramref name="condition"/> is true; otherwise, the original sequence.</returns>
    public static IQueryable<T> OrderByIf<T, TKey>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, TKey>> orderBy)
    {
        return condition ? query.OrderBy(orderBy) : query;
    }

    /// <summary>
    /// Conditionally applies a descending order to the query if the specified condition is true.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key to order by.</typeparam>
    /// <param name="query">The source queryable sequence.</param>
    /// <param name="condition">If true, the ordering is applied; otherwise, the source is returned unchanged.</param>
    /// <param name="orderBy">The key selector expression for ordering.</param>
    /// <returns>The ordered queryable sequence in descending order if <paramref name="condition"/> is true; otherwise, the original sequence.</returns>
    public static IQueryable<T> OrderByDescendingIf<T, TKey>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, TKey>> orderBy)
    {
        return condition ? query.OrderByDescending(orderBy) : query;
    }

    /// <summary>
    /// Returns a specific page of results from the queryable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <param name="query">The source queryable sequence.</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A queryable sequence containing the specified page of results.</returns>
    public static IQueryable<T> GetPage<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Returns distinct elements from a sequence by using a specified key selector.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key to distinguish elements.</typeparam>
    /// <param name="query">The source queryable sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>A queryable sequence that contains distinct elements based on the specified key.</returns>
    public static IQueryable<TSource> DistinctBy<TSource, TKey>(
        this IQueryable<TSource> query,
        Expression<Func<TSource, TKey>> keySelector)
    {
        return query.GroupBy(keySelector).Select(x => x.First());
    }
}
