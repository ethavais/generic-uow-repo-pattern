using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace AppRepos.Core
{
    public interface IQueryRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> CoreQuery(
            bool disableTracking = true,
            bool ignoreQueryFilters = false);

        IQueryable<TEntity> BaseQuery(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);

        IQueryable<TResult> SelectQuery<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);

        ValueTask<TEntity?> FindByIdAsync(object keyValue);
        Task<TEntity?> FindByKeysAsync(params object[] keysValue);
        Task<TEntity?> FindByKeysAsync(object[] keysValues, CancellationToken cancellationToken);

        #region Synchronous Version
        TEntity? FindById(object keyValue);
        TEntity? FindByKeys(params object[] keysValue);
        TEntity? FindByKeys(object[] keysValues, CancellationToken cancellationToken);
        #endregion
    }


    //public class QueryRepository<TEntity> : BaseRepository<TEntity>, IQueryRepository<TEntity> where TEntity : class
    //{
    //    public QueryRepository(DbContext dbContext) : base(dbContext) { }

    //    public IQueryable<TEntity> CoreQuery(
    //        bool disableTracking = true,
    //        bool ignoreQueryFilters = false)
    //    {
    //        IQueryable<TEntity> query = _dbSet;

    //        query = disableTracking ? query.AsNoTracking() : query;
    //        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

    //        return query;
    //    }

    //    public IQueryable<TEntity> BaseQuery(
    //        Expression<Func<TEntity, bool>>? predicate = null,
    //        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    //        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    //        bool disableTracking = true,
    //        bool ignoreQueryFilters = false)
    //    {
    //        IQueryable<TEntity> query = CoreQuery(disableTracking, ignoreQueryFilters);

    //        query = query
    //            .ApplyIncludes(include)
    //            .ApplyPredicate(predicate)
    //            .ApplyOrdering(orderBy);

    //        return query;
    //    }

    //    public IQueryable<TResult> SelectQuery<TResult>(
    //        Expression<Func<TEntity, TResult>> selector,
    //        Expression<Func<TEntity, bool>>? predicate = null,
    //        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    //        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    //        bool disableTracking = true,
    //        bool ignoreQueryFilters = false)
    //    {
    //        IQueryable<TEntity> query = BaseQuery(predicate, orderBy, include, disableTracking, ignoreQueryFilters);

    //        return query.ApplySelect(selector);
    //    }

    //    public virtual ValueTask<TEntity?> FindByIdAsync(object keyValue)
    //        => _dbSet.FindAsync(keyValue);

    //    public virtual Task<TEntity?> FindByKeysAsync(params object[] keysValue)
    //        => _dbSet.FindAsync(keysValue).AsTask();

    //    public virtual Task<TEntity?> FindByKeysAsync(object[] keysValues, CancellationToken cancellationToken)
    //        => _dbSet.FindAsync(keysValues, cancellationToken).AsTask();

    //}
}
