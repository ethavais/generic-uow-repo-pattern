using AppRepos.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace AppRepos
{
    public interface IRepository<TEntity> :
        IBaseRepository<TEntity>,
        IQueryRepository<TEntity>,
        ICommandRepository<TEntity>,
        IAggregationRepository<TEntity>
        where TEntity : class
    {
    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
        }

        #region Base Repository
        public virtual void ChangeTable(string table)
        {
            if (_dbContext.Model.FindEntityType(typeof(TEntity)) is IConventionEntityType relational)
                relational.SetTableName(table);
        }

        public void ChangeEntityState(TEntity entity, EntityState state)
            => _dbContext.Entry(entity).State = state;

        public IQueryable<TEntity> FromSql(string sql, params object[] parameters)
            => _dbSet.FromSqlRaw(sql, parameters);
        #endregion



        #region Query Repository
        public IQueryable<TEntity> CoreQuery(
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            query = disableTracking ? query.AsNoTracking() : query;
            query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

            return query;
        }

        public IQueryable<TEntity> BaseQuery(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = CoreQuery(disableTracking, ignoreQueryFilters);

            query = query
                .ApplyIncludes(include)
                .ApplyPredicate(predicate)
                .ApplyOrdering(orderBy);

            return query;
        }

        public IQueryable<TResult> SelectQuery<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = BaseQuery(predicate, orderBy, include, disableTracking, ignoreQueryFilters);

            return query.ApplySelect(selector);
        }

        public virtual ValueTask<TEntity?> FindByIdAsync(object keyValue)
            => _dbSet.FindAsync(keyValue);

        public virtual Task<TEntity?> FindByKeysAsync(params object[] keysValue)
            => _dbSet.FindAsync(keysValue).AsTask();

        public virtual Task<TEntity?> FindByKeysAsync(
            object[] keysValues, 
            CancellationToken cancellationToken)
            => _dbSet.FindAsync(keysValues, cancellationToken).AsTask();
        #endregion

        #region Command Repository
        public virtual ValueTask<EntityEntry<TEntity>> InsertAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
            => _dbSet.AddAsync(entity, cancellationToken);

        public virtual Task InsertAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
            => _dbSet.AddRangeAsync(entities, cancellationToken);

        public virtual void Update(TEntity entity)
            => _dbSet.Update(entity);

        public virtual void Update(IEnumerable<TEntity> entities)
            => _dbSet.UpdateRange(entities);

        public virtual void Delete(TEntity entity)
            => _dbSet.Remove(entity);

        public virtual void Delete(IEnumerable<TEntity> entities)
            => _dbSet.RemoveRange(entities);

        public virtual void Delete(object id)
        {
            var entity = _dbSet.Find(id);

            if (entity != null)
                Delete(entity);
        }
        #endregion

        #region Aggregation Repository
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            var query = CoreQuery().ApplyPredicate(predicate);
            return await query.CountAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null)
        {
            var query = CoreQuery().ApplyPredicate(selector);
            return await query.AnyAsync();
        }

        public async Task<T> MaxAsync<T>(
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, T>>? selector = null)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var query = CoreQuery().ApplyPredicate(predicate);
            return await query.MaxAsync(selector);
        }

        public async Task<T> MinAsync<T>(
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, T>>? selector = null)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var query = CoreQuery().ApplyPredicate(predicate);
            return await query.MinAsync(selector);
        }
        #endregion



        #region Query Synchronous
        public virtual TEntity? FindById(object keyValue)
            => _dbSet.Find(keyValue);

        public virtual TEntity? FindByKeys(params object[] keysValue)
            => _dbSet.Find(keysValue);

        public virtual TEntity? FindByKeys(
            object[] keysValues,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException(cancellationToken);

            return _dbSet.Find(keysValues);
        }

        #endregion

        #region Command Synchronous
        public virtual EntityEntry<TEntity> Insert(TEntity entity)
            => _dbSet.Add(entity);

        public virtual void Insert(IEnumerable<TEntity> entities)
            => _dbSet.AddRange(entities);
        #endregion

        #region Aggregation Synchronous 
        public int Count(Expression<Func<TEntity, bool>>? predicate = null)
        {
            var query = CoreQuery().ApplyPredicate(predicate);
            return query.Count();
        }

        public bool Exists(Expression<Func<TEntity, bool>>? selector = null)
        {
            var query = CoreQuery().ApplyPredicate(selector);
            return query.Any();
        }

        public T Max<T>(
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, T>>? selector = null)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var query = CoreQuery().ApplyPredicate(predicate);
            return query.Max(selector);

        }

        public T Min<T>(
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, T>>? selector = null)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var query = CoreQuery().ApplyPredicate(predicate);
            return query.Min(selector);
        }
        #endregion
    }


}
