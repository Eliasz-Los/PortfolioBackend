using AutoMapper;
using BL.DocuGroup.Dto.Component;
using BL.DocuGroup.Dto.Draft;
using Domain.DocuGroup;

namespace BL.DocuGroup.Mapper;

public class ComponentMappingProfile : Profile
{
    public ComponentMappingProfile()
    {
        CreateMap<DocumentComponent, ComponentDto>();
        CreateMap<ComponentDto, DocumentComponent>();
        
        CreateMap<DraftComponent, DocumentComponent>();
        CreateMap<DocumentComponent, DraftComponent>();
    }
}