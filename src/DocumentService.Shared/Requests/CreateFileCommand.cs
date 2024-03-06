using System.ComponentModel.DataAnnotations;

namespace DocumentService.Shared.Requests;

/// <summary>
/// Запрос для создания файла
/// </summary>
/// <param name="FolderId">Идентификатор папки, в которой будет рамзещён файл</param>
/// <param name="FileName">Название файла</param>
/// <param name="FileLength">Размер файла</param>
/// <param name="ContentType">Тип контента</param>
public sealed record CreateFileCommand(
    [Required] string FileName,
    [Required] long FileLength,
    [Required] string ContentType);
