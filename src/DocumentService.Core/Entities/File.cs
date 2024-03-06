namespace DocumentService.Core.Entities;

/// <summary>
/// Документ
/// </summary>
public class File : EntityBase<Guid>
{
    private File() { }

    public File(string name, string? description, string bucketKey, long fileSize, Guid ownerId)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(bucketKey);

        if (fileSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(fileSize), "File size must be positive");

        Name = name;
        BucketKey = bucketKey;
        Description = description;
        FileSize = fileSize;

        CreatedAt = DateTime.UtcNow;
    }
    /// <summary>
    /// Имя файла
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// Описание файла
    /// </summary>
    public string? Description { get; private set; }
    /// <summary>
    /// Ключ в хранилище
    /// </summary>
    public string BucketKey { get; private set; }
    /// <summary>
    /// Размер файлав байтах
    /// </summary>
    public long FileSize { get; private set; }
    /// <summary>
    /// Признак удаления
    /// </summary>
    public bool IsDeleted { get; private set; }
    /// <summary>
    /// Дата cоздания файла
    /// </summary>
    public DateTime CreatedAt { get; private set; }
}
