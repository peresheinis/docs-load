using Amazon.S3;
using Amazon.S3.Model;
using DocumentService.Core.Repositories;
using DocumentService.Shared.Responses.Files;
using DocumentService.Web.Configurations;
using DocumentService.Web.Exceptions;
using DocumentService.Web.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace DocumentService.Web.Application.Commands;

public static class DownloadFile
{
    /// <summary>
    /// Команда для скачивания файла
    /// </summary>
    /// <param name="FileId"></param>
    public record Command(Guid FileId) : IRequest<DownloadFileDto>;
    /// <summary>
    /// Хандлер для скачивания файла
    /// </summary>
    internal class Handler : IRequestHandler<Command, DownloadFileDto>
    {
        private readonly AmazonS3Client _client;
        private readonly IFilesRepository _filesRepository;
        private readonly IProductionService _productionService;
        private readonly IOptionsMonitor<StorageConfiguration> _storageConfiguration;

        public Handler(
            AmazonS3Client amazonS3Client,
            IFilesRepository filesRepository,
            IProductionService productionService,
            IOptionsMonitor<StorageConfiguration> storageConfiguration)
        {
            _client = amazonS3Client;
            _filesRepository = filesRepository;
            _productionService = productionService;
            _storageConfiguration = storageConfiguration;
        }
        public async Task<DownloadFileDto> Handle(Command request, CancellationToken cancellationToken)
        {
            File file = (await _filesRepository.GetByIdAsync(request.FileId)) ?? throw ConflictException.NotFound("Файл не найден");

            return new DownloadFileDto(
                CreatePresignedUrl(file.BucketKey, HttpVerb.GET),
                CreatePresignedUrl(file.BucketKey, HttpVerb.HEAD),
                file.Name);
        }
        /// <summary>
        /// Сгенерировать подписанный Url для загрузки / просмотра файла
        /// </summary>
        /// <param name="bucketKey"></param>
        /// <returns></returns>
        private string CreatePresignedUrl(string bucketKey, HttpVerb httpVerb)
        {
            GetPreSignedUrlRequest request = new()
            {
                Verb = httpVerb,
                Key = bucketKey,
                BucketName = _storageConfiguration.CurrentValue.Bucket,
                Expires = DateTime.Now.AddMinutes(StorageConfiguration.PresignedUrlExpiresInMinutes)
            };

            return _client.GetPreSignedURL(request);
        }
    }
}