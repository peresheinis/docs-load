using DocumentService.Core.Repositories;
using DocumentService.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Infrastructure.Repositories;

public class FilesRepository : RepositoryBase<File>, IFilesRepository
{
    public FilesRepository(DatabaseContext databaseContext) : base(databaseContext.Files)
    {
    }

    public async Task<File> Add(File entity) =>
        (await DbSet.AddAsync(entity)).Entity;

    public Task<File?> GetByIdAsync(Guid id) =>
         ApplySpecification(new FileByIdSpecification(id))
              .FirstOrDefaultAsync();

    public void Remove(File entity) => 
        DbSet.Remove(entity);

    public File Update(File entity) =>
        DbSet.Update(entity).Entity;
}
