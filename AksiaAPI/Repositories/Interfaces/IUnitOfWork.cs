namespace AksiaAPI.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        ITransactionRepository TransactionRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
