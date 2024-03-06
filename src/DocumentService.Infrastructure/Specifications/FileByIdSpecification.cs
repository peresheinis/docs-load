using Microsoft.EntityFrameworkCore;

namespace DocumentService.Infrastructure.Specifications;

/// <summary>
/// Спецификация для поиска файла по идентификатор и идентификатору производства
/// </summary>
internal class FileByIdSpecification : SpecificationBase<File>
{
    public FileByIdSpecification(Guid fileId)
        : base(file => file.Id == fileId)
    {
    }
}