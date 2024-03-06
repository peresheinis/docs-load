using AutoMapper;
using DocumentService.Shared.Responses.Files;

namespace DocumentService.Web.Configurations;

public class MapperEntitiesProfile : Profile
{
    public MapperEntitiesProfile()
    {
        this.CreateMap<File, FileDto>();
    }
}
