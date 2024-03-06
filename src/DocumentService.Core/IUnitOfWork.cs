namespace DocumentService.Core
{
    public interface IUnitOfWork
    {
        public Task<int> SaveChangesAsync();
    }
}
