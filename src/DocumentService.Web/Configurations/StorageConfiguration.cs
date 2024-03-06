namespace DocumentService.Web.Configurations;

public class StorageConfiguration
{
    /// <summary>
    /// Время истечения подписанного URL, ToDo: 
    /// Генерировать примерное значение из размера файла, 
    /// для возможности загрузки без прерывания
    /// </summary>
    public const int PresignedUrlExpiresInMinutes = 10;
    public const string Storage = "Storage";
    /// <summary>
    /// Ключ доступа
    /// </summary>
    public string AccessKey { get; set; }
    /// <summary>
    /// Секретный ключ
    /// </summary>
    public string SecretKey { get; set; }
    /// <summary>
    /// Регион хранилища
    /// </summary>
    public string Region { get; set; }
    /// <summary>
    /// Имя бакета
    /// </summary>
    public string Bucket { get; set; }
    /// <summary>
    /// Url хранилища
    /// </summary>
    public string Url { get; set; }
}
