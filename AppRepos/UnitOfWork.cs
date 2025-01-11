using Microsoft.EntityFrameworkCore;

namespace AppRepos
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        TContext DbContext { get; }

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        #region SaveChanges
        Task<int> SaveChangesAsync();

        int SaveChanges();
        #endregion
    }
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly TContext _context;
        private Dictionary<Type, object> repositories;
        private bool disposed = false;

        public TContext DbContext => _context;

        public UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            repositories = new Dictionary<Type, object>();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (repositories == null)
                repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new Repository<TEntity>(_context);
            }

            return (IRepository<TEntity>)repositories[type];
        }

        #region SaveChanges
        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public int SaveChanges()
            => _context.SaveChanges();
        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (repositories != null)
                        repositories.Clear();

                    _context.Dispose();
                }
            }

            disposed = true;
        }
        #endregion
    }
}
