using DocumentService.Core;
using DocumentService.Core.Entities;
using DocumentService.Core.Repositories;
using DocumentService.Web.Exceptions;
using DocumentService.Web.Services;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace DocumentService.Web.Application.Commands;

public static class DeleteFile
{
    public record Command([Required] Guid FileId) : IRequest;
    internal class Handler : IRequestHandler<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFilesRepository _filesRepository;
        private readonly IProductionService _productionService;
        private readonly IFoldersRepository _foldersRepository;

        public Handler(
            IProductionService productionService,
            IFoldersRepository foldersRepository,
            IFilesRepository filesRepository, IUnitOfWork unitOfWork)
        {
            _productionService = productionService;
            _foldersRepository = foldersRepository;
            _filesRepository = filesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            File file = (await _filesRepository.GetByIdAsync(request.FileId)) ?? throw ConflictException.NotFound("Файл не найден");

            file.SetDeleted(_productionService.UserId);

            _filesRepository.Update(file);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}