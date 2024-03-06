using Amazon.S3;
using Amazon.S3.Model;
using DocumentService.Core.Repositories;
using DocumentService.Shared.Responses.Errors;
using DocumentService.Web.Configurations;
using DocumentService.Web.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DocumentService.Web.Controllers
{
    public class BucketsController : ApiControllerBase
    {
        private readonly AmazonS3Client _amazonS3Client;
        private readonly IOptionsMonitor<StorageConfiguration> _selectel;
        private readonly IFilesRepository _filesRepository;

        public BucketsController(
            AmazonS3Client amazonS3Client,
            IOptionsMonitor<StorageConfiguration> selectel,
            IFilesRepository filesRepository)
        {
            _selectel = selectel;
            _amazonS3Client = amazonS3Client;
            _filesRepository = filesRepository;
        }
        /// <summary>
        /// Получить подписанный Url для скачивания / просмотра файла
        /// </summary>
        /// <param name="objectKey">Ключ объекта</param>
        /// <param name="expiryMinutes">Время истечения в минутах</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> GetDownloadPresignedUrl(Guid fileId)
        {
            File file = (await _filesRepository.GetByIdAsync(fileId)) ?? throw ConflictException.NotFound("Файл не найден.");

            GetPreSignedUrlRequest request = new()
            {
                Verb = HttpVerb.GET,
                Key = file.BucketKey,
                BucketName = _selectel.CurrentValue.Bucket,
                Expires = DateTime.Now.AddMinutes(StorageConfiguration.PresignedUrlExpiresInMinutes)
            };

            return _amazonS3Client.GetPreSignedURL(request);
        }
    }
}
