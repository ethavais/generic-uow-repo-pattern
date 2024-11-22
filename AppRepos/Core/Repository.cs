using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRepos.Core
{
    public interface IRepository<TEntity> :
        IQueryRepository<TEntity>,
        ICommandRepository<TEntity>,
        IPagedRepository<TEntity>,
        IAggregationRepository<TEntity>
        where TEntity : class
    {
        void ChangeTable(string table);
        void ChangeEntityState(TEntity entity, EntityState state);
    }

}
