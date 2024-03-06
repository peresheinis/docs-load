namespace DocumentService.Shared.Responses.Files;

public record UploadFileDto(Guid FileId, string PresignedUrl);
