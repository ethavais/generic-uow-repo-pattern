using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRepos.Core
{
    public interface IRepository<TEntity> :
        IBaseRepository<TEntity>,
        IQueryRepository<TEntity>,
        ICommandRepository<TEntity>,
        IAggregationRepository<TEntity>
        where TEntity : class
    {
    }

    public class Repository<TEntity> :
    BaseRepository<TEntity>,
    QueryRepository<TEntity>,
    CommandRepository<TEntity>,
    AggregationRepository<TEntity>,
    IRepository<TEntity>
    where TEntity : class
    {
        public Repository(DbContext dbContext) : base(dbContext) { }
    }


}
