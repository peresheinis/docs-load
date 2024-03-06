namespace DocumentService.Shared.Requests;

/// <summary>
/// Команда для обновления файла
/// </summary>
/// <param name="Name">Новое имя файла</param>
/// <param name="Description">Новое описание файла</param>
/// <param name="FolderId">Новая родительская папка</param>
public sealed record UpdateFileCommand(
    string? Name, string? Description);