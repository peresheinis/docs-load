namespace DocumentService.Core.Repositories.Base;

public interface IWriteRepository<TEntity>
{
    Task<TEntity> Add(TEntity entity);
    TEntity Update(TEntity entity);
    void Remove(TEntity entity);
}
