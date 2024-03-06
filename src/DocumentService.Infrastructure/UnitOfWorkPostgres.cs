using DocumentService.Core;

namespace DocumentService.Infrastructure
{
    public class UnitOfWorkPostgres : IUnitOfWork   
    {
        private readonly DatabaseContext _dbContext;
        public UnitOfWorkPostgres(DatabaseContext databaseContext)  => _dbContext = databaseContext;
        public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();   
    }
}
