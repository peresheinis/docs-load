using DocumentService.Core.Repositories.Base;

namespace DocumentService.Core.Repositories;

public interface IWriteFilesRepository : IWriteRepository<File>
{
}

public interface IReadFilesRepository : IReadRepository<File, Guid>
{
    
}

public interface IFilesRepository : IReadFilesRepository, IWriteFilesRepository
{
}