namespace DocumentService.Shared.Responses.Files
{
    public class FileDto
    { 
        public Guid Id { get; set; }
        /// <summary>
        /// Название файла
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Описание файла
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// Время создания
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Последняя дата изменения
        /// </summary>
        public DateTime EditedAt { get; set; }
    }
}
