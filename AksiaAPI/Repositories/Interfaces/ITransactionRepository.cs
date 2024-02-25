using AksiaAPI.Models.Business;
using AksiaAPI.Models.Entities;

namespace AksiaAPI.Repositories.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetPagedAsync(Page page);
    }
}
