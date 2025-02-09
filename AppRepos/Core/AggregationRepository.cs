﻿using System.Linq.Expressions;

namespace AppRepos.Core
{
    public interface IAggregationRepository<TEntity> where TEntity : class
    {
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null);


        Task<T> MaxAsync<T>(
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, T>>? selector = null);
        Task<T> MinAsync<T>(
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, T>>? selector = null);

        #region Synchronous Version 
        int Count(Expression<Func<TEntity, bool>>? predicate = null);
        bool Exists(Expression<Func<TEntity, bool>>? selector = null);

        T Max<T>(
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, T>>? selector = null);
        T Min<T>(
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, T>>? selector = null);
        #endregion
    }

    //public class AggregationRepository<TEntity> : QueryRepository<TEntity>, IAggregationRepository<TEntity> where TEntity : class
    //{
    //    public AggregationRepository(DbContext dbContext) : base(dbContext) { }

    //    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
    //    {
    //        var query = CoreQuery().ApplyPredicate(predicate);
    //        return await query.CountAsync();
    //    }

    //    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null)
    //    {
    //        var query = CoreQuery().ApplyPredicate(selector);
    //        return await query.AnyAsync();
    //    }

    //    public async Task<T> MaxAsync<T>(
    //        Expression<Func<TEntity, bool>>? predicate = null,
    //        Expression<Func<TEntity, T>>? selector = null)
    //    {
    //        if (selector == null) throw new ArgumentNullException(nameof(selector));

    //        var query = CoreQuery().ApplyPredicate(predicate);
    //        return await query.MaxAsync(selector);
    //    }

    //    public async Task<T> MinAsync<T>(
    //        Expression<Func<TEntity, bool>>? predicate = null,
    //        Expression<Func<TEntity, T>>? selector = null)
    //    {
    //        if (selector == null) throw new ArgumentNullException(nameof(selector));

    //        var query = CoreQuery().ApplyPredicate(predicate);
    //        return await query.MinAsync(selector);
    //    }
    //}


}
