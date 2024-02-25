using Microsoft.EntityFrameworkCore;

namespace AksiaAPI.Repositories
{
    public abstract class Repository<T>  where T : class
    {
        protected AksiaDbContext DbContext { get; }

        protected Repository(AksiaDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual T Add(T entity)
        {
            var change = DbContext.Set<T>().Add(entity);
            return change.Entity;
        }

        public virtual IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            DbContext.Set<T>().AddRange(entities);
            return entities;
        }

        public virtual void Delete(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }

        public virtual async Task<T> FindAsync(params object[] keyValues)
        {
            return await DbContext.Set<T>().FindAsync(keyValues);
        }

        public virtual T Update(T entity)
        {
            DbContext.Attach(entity);
            DbContext.Set<T>().Update(entity);
            return entity;
        }

        public virtual IEnumerable<T> UpdateRange(IEnumerable<T> entities)
        {
            DbContext.Set<T>().AttachRange(entities);
            DbContext.Set<T>().UpdateRange(entities);
            return entities;
        }
    }
}
