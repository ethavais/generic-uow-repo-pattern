using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace AppRepos
{
    public static class QueryableExtensions
    {
        public static IQueryable<TResult> ApplySelect<T, TResult>(
            this IQueryable<T> query,
            Expression<Func<T, TResult>> selector)
        => query.Select(selector);

        public static IQueryable<TEntity> ApplyPredicate<TEntity>(
            this IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>>? predicate)
        => predicate != null ? query.Where(predicate) : query;

        public static IQueryable<TEntity> ApplyIncludes<TEntity>(
            this IQueryable<TEntity> query, Func<IQueryable<TEntity>,
            IIncludableQueryable<TEntity, object>>? include)
        => include != null ? include(query) : query;

        public static IQueryable<TEntity> ApplyOrdering<TEntity>(
            this IQueryable<TEntity> query,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy)
        => orderBy != null ? orderBy(query) : query;
    }
}
