using AksiaAPI.Models.Business;
using AksiaAPI.Models.Entities;
using AksiaAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AksiaAPI.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AksiaDbContext dbContext)
            : base(dbContext)
        {

        }

        public async Task<IEnumerable<Transaction>> GetPagedAsync(Page page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var query = DbContext.Transactions.OrderBy(x => x.Inception);

            return await query
                .Skip((page.PageIndex - 1) * page.PageSize)
                .Take(page.PageSize)
                .ToListAsync();
        }
    }
}
