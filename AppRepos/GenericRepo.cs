using AppBOs.Kernels;
using AppBOs;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppRepos
{
    public class GenericRepo<T> where T : class
    {
        protected Fall24PharmaceuticalDbContext GetDbScope()
        {
            return new Fall24PharmaceuticalDbContext();
        }

        private void LogAction(string action, string status, bool isEndProcess = false)
        {
            string log = $"{status} {action} for {typeof(T).Name} entity.";

            if (isEndProcess)
                log += "\n";

            Console.WriteLine(log);
        }

        public bool IsValid(T entity, string action)
        {
            if (entity is IValidatable validatableEntity)
            {
                if (!validatableEntity.Validate(out string validationError))
                {
                    LogAction(action, $"Failed. Validation error: {validationError}", true);
                    return false;
                }
            }

            return true;
        }

        private IQueryable<T> AddIncludes(DbContext dbContext, IQueryable<T> query)
        {
            var navigationProperties = dbContext.Model
                .FindEntityType(typeof(T))?
                .GetNavigations()
                .Select(n => n.Name);

            if (navigationProperties != null)
            {
                foreach (var navigationProperty in navigationProperties)
                {
                    query = query.Include(navigationProperty);
                }
            }
            return query;
        }



        public async Task<List<T>> GetAllAsync(bool isIncludeAll = false)
        {
            string action = isIncludeAll ? "retrieving all with includes" : "retrieving all";
            try
            {
                using var dbContext = GetDbScope();
                IQueryable<T> query = dbContext.Set<T>();

                if (isIncludeAll)
                    query = AddIncludes(dbContext, query);

                var result = await query.ToListAsync();
                LogAction(action, $"<Completed>\n \t Retrieved {result.Count} items" + (isIncludeAll ? " with includes" : ""), true);

                return result;
            }
            catch (Exception ex)
            {
                LogAction(action, $"Failed. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<object?> GetMaxIdAsync(Expression<Func<T, object>> idSelector)
        {
            string action = "retrieving max ID";
            try
            {
                using var dbContext = GetDbScope();
                IQueryable<T> query = dbContext.Set<T>();

                var maxId = await query.MaxAsync(idSelector);

                LogAction(action, $"<Completed>\n \t Max ID retrieved: {maxId}", true);
                return maxId;
            }
            catch (Exception ex)
            {
                LogAction(action, $"Failed. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<T?> GetByKeyAsync(Expression<Func<T, bool>> predicate, bool isIncludeAll = false)
        {
            string action = $"retrieving entity with predicate: {predicate.Body}" + (isIncludeAll ? " with includes" : "");
            try
            {
                using var dbContext = GetDbScope();
                IQueryable<T> query = dbContext.Set<T>();

                if (isIncludeAll)
                    query = AddIncludes(dbContext, query);

                var result = await query.FirstOrDefaultAsync(predicate);

                LogAction(action, result != null ? "Completed successfully" : "No match found", true);

                return result;
            }
            catch (Exception ex)
            {
                LogAction(action, $"Failed. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> InsertAsync(T entity)
        {
            string action = "inserting";
            try
            {
                if (!IsValid(entity, action))
                    return false;

                using var dbContext = GetDbScope();
                await dbContext.Set<T>().AddAsync(entity);
                var result = await dbContext.SaveChangesAsync() > 0;
                LogAction(action, result ? "Completed successfully" : "Failed", true);

                return result;
            }
            catch (Exception ex)
            {
                LogAction(action, $"Failed. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            string action = "updating";
            try
            {
                if (!IsValid(entity, action))
                    return false;

                LogAction(action, "Starting");
                using var dbContext = GetDbScope();
                dbContext.Set<T>().Update(entity);
                var result = await dbContext.SaveChangesAsync() > 0;
                LogAction(action, result ? "Completed successfully" : "Failed", true);

                return result;
            }
            catch (Exception ex)
            {
                LogAction(action, $"Failed. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            string action = "deleting";
            try
            {
                using var dbContext = GetDbScope();
                dbContext.Set<T>().Remove(entity);
                var result = await dbContext.SaveChangesAsync() > 0;
                LogAction(action, result ? "Completed successfully" : "Failed", true);

                return result;
            }
            catch (Exception ex)
            {
                LogAction(action, $"Failed. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<T>> GetPagedAsync(int pageNumber, int pageSize, List<T>? existingData = null)
        {
            string action = "retrieving paged results";
            try
            {
                List<T> result;

                if (existingData != null)
                {
                    result = existingData
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }
                else
                {
                    using var dbContext = GetDbScope();
                    var query = dbContext.Set<T>().AsQueryable();

                    result = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                }

                LogAction(action, $"<Completed>\n \t Retrieved {result.Count} results", true);

                return result;
            }
            catch (Exception ex)
            {
                LogAction(action, $"Failed. Error: {ex.Message}");
                throw;
            }
        }
    }

}
