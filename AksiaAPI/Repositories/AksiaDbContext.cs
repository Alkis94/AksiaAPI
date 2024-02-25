using AksiaAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace AksiaAPI.Repositories
{
    public class AksiaDbContext: DbContext
    {
        public AksiaDbContext(DbContextOptions<AksiaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
