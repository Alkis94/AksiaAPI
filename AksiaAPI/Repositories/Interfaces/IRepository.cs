namespace AksiaAPI.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Add(TEntity entity);

        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);

        void Delete(TEntity entity);

        Task<TEntity> FindAsync(params object[] keyValues);

        TEntity Update(TEntity entity);

        IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities);
    }
}
