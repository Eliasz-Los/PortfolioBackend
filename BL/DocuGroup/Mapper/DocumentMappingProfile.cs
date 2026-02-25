using AutoMapper;
using BL.DocuGroup.Dto.Document;
using BL.DocuGroup.Dto.Draft;
using Domain.DocuGroup;

namespace BL.DocuGroup.Mapper;

public class DocumentMappingProfile : Profile
{
    public DocumentMappingProfile()
    {
        CreateMap<DocumentDto, GroupDocument>();
        CreateMap<GroupDocument, DocumentDto>();
        CreateMap<DocumentDetailsDto, GroupDocument>();
        CreateMap<GroupDocument, DocumentDetailsDto>();

        CreateMap<DraftDocument, GroupDocument>();
        CreateMap<GroupDocument, DraftDocument>();
    }
}