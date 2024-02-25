using AksiaAPI.Repositories.Interfaces;

namespace AksiaAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AksiaDbContext _dbContext;
        private ITransactionRepository _transactionRepository;

        public UnitOfWork(AksiaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ITransactionRepository TransactionRepository
        {
            get => _transactionRepository ??= new TransactionRepository(
                _dbContext);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
