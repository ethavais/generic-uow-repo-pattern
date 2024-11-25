using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace AppRepos.Core
{
    public interface ICommandRepository<TEntity> where TEntity : class
    {
        ValueTask<EntityEntry<TEntity>> InsertAsync(
            TEntity entity, 
            CancellationToken cancellationToken = default);
        Task InsertAsync(
            IEnumerable<TEntity> entities, 
            CancellationToken cancellationToken = default);

        void Update(TEntity entity);
        void Update(IEnumerable<TEntity> entities);

        void Delete(object id);
        void Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entities);
    }

    public class CommandRepository<TEntity> : BaseRepository<TEntity>, ICommandRepository<TEntity> where TEntity : class
    {
        public CommandRepository(DbContext dbContext) : base(dbContext) { }

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


    }

}
