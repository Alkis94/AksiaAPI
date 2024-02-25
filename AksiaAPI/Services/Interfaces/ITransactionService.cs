using AksiaAPI.Models.Business;
using AksiaAPI.Models.Entities;

namespace AksiaAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> Get(Guid id);

        Task<IEnumerable<Transaction>> GetPagedAsync(Page page);

        Task<Guid> Insert(TransactionInsertOrUpdate transactionInsert);

        Task Delete(Guid id);

        Task<string> ParseCSV(IFormFile file);
    }
}
