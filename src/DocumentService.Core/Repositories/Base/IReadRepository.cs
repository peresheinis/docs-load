namespace DocumentService.Core.Repositories.Base;

public interface IReadRepository<TEntity, TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id);
}
