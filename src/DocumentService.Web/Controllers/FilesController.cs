using AutoMapper;
using DocumentService.Core.Repositories;
using DocumentService.Shared.Requests;
using DocumentService.Shared.Responses.Files;
using DocumentService.Web.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DocumentService.Web.Controllers;

/// <summary>
/// API для управления файлами
/// </summary>
public class FilesController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IFilesRepository _filesRepository;

    public FilesController(
        IMapper mapper,
        IMediator mediator, 
        IFilesRepository filesRepository) : base()
    {
        _mapper = mapper;
        _mediator = mediator;
        _filesRepository = filesRepository;
    }

    /// <summary>
    /// Создать файл
    /// </summary>
    /// <returns>PresignedUrl для загрузки файла</returns>
    [HttpPost]
    public async Task<ActionResult<UploadFileDto>> CreateFile(
        CreateFileCommand request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateFile.Command(
            request.FileName,
            request.FileLength,
            request.ContentType);

        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Удалить файл
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public Task DeleteFile(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteFile.Command(id);

        return _mediator.Send(command, cancellationToken);
    }


    [HttpGet("{fileId:guid}")]
    public async Task<ActionResult<FileDto>> GetFile(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        var file = await _filesRepository.GetByIdAsync(fileId);
        var fileDto = _mapper.Map<FileDto>(file);

        return fileDto;
    }

    /// <summary>
    /// Получить подписанный Url для скачивания файла
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns>PresignedUrl для скачивания файла</returns>
    [HttpGet("{fileId:guid}/Download")]
    public async Task<ActionResult<DownloadFileDto>> DownloadFile(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        DownloadFile.Command command = new(fileId);

        return await _mediator.Send(command, cancellationToken);
    }
}
