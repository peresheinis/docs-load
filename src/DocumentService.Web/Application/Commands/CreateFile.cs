using Amazon.S3;
using Amazon.S3.Model;
using DocumentService.Core;
using DocumentService.Core.Entities;
using DocumentService.Core.Repositories;
using DocumentService.Shared.Responses.Files;
using DocumentService.Web.Configurations;
using DocumentService.Web.Exceptions;
using DocumentService.Web.Services;
using MediatR;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace DocumentService.Web.Application.Commands;

/// <summary>
/// Команда для создания файла
/// </summary>
public static class CreateFile
{
    /// <summary>
    /// Команда для создания файла
    /// </summary>
    /// <param name="FolderId"></param>
    /// <param name="FileName"></param>
    /// <param name="FileLength"></param>
    /// <param name="ContentType"></param>
    public record Command(
        [Required] string FileName,
        [Required] long FileLength,
        [Required] string ContentType) : IRequest<UploadFileDto>;

    /// <summary>
    /// Реализация хандлера команды <see cref="Command"/>
    /// </summary>
    internal class Handler : IRequestHandler<Command, UploadFileDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductionService _productionService;
        private readonly IOptionsMonitor<StorageConfiguration> _storage;

        private readonly AmazonS3Client _s3Client;

        public Handler(
            IProductionService productionService,
            IUnitOfWork unitOfWork,
            IOptionsMonitor<StorageConfiguration> storage,
            AmazonS3Client amazonS3Client)
        {
            _storage = storage;
            _s3Client = amazonS3Client;
            _unitOfWork = unitOfWork;
            _productionService = productionService;
        }

        public async Task<UploadFileDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var requestedFolder = (await _foldersRepository.GetByIdAsync(request.FolderId, _productionService.ProductionId)) ?? throw ConflictException.NotFound("Папка не найдена");
            var documentBucketKey = string.Format("{0}\\{1}{2}", _productionService.ProductionId, Guid.NewGuid(), Path.GetExtension(request.FileName));
            var file = requestedFolder.AddFile(IncrementFileName(requestedFolder, request.FileName), null, documentBucketKey, request.FileLength, _productionService.UserId);

            _foldersRepository.Update(requestedFolder);

            var changes = await _unitOfWork.SaveChangesAsync();
            var presignedUrl = CreatePresignedUrl(file.BucketKey, request.ContentType);
            var uploadDto = new UploadFileDto(file.Id, presignedUrl);

            return uploadDto;
        }

        /// <summary>
        /// Получить уникальное имя файла при загрузке
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string IncrementFileName(Folder folder, string fileName)
        {
            string baseFileName = Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = Path.GetExtension(fileName);
            string uniqueFileName = fileName;
            int counter = 1;

            while (folder.Documents.Any(d => string.Equals(d.Name, uniqueFileName, StringComparison.OrdinalIgnoreCase)))
            {
                uniqueFileName = $"{baseFileName} ({counter}){fileExtension}";
                counter++;
            }

            return uniqueFileName;
        }

        /// <summary>
        /// Создать подписанный Url для загрузки файла
        /// </summary>
        /// <param name="bucketKey"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private string CreatePresignedUrl(string bucketKey, string contentType)
        {
            GetPreSignedUrlRequest request = new()
            {
                Verb = HttpVerb.PUT,
                Protocol = Protocol.HTTPS,
                Key = bucketKey,
                BucketName = _storage.CurrentValue.Bucket,
                ContentType = contentType,
                Expires = DateTime.Now.AddMinutes(StorageConfiguration.PresignedUrlExpiresInMinutes)
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }
}