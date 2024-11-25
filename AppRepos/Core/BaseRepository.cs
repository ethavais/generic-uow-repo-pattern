using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AppRepos.Core
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void ChangeTable(string table);
        void ChangeEntityState(TEntity entity, EntityState state);
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);
    }

    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseRepository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
        }

        public virtual void ChangeTable(string table)
        {
            if (_dbContext.Model.FindEntityType(typeof(TEntity)) is IConventionEntityType relational)
                relational.SetTableName(table);
        }

        public void ChangeEntityState(TEntity entity, EntityState state)
            => _dbContext.Entry(entity).State = state;

        public IQueryable<TEntity> FromSql(string sql, params object[] parameters)
            => _dbSet.FromSqlRaw(sql, parameters);
    }
}
