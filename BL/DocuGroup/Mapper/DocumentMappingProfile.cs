using AutoMapper;
using BL.DocuGroup.Dto.Document;
using Domain.DocuGroup;

namespace BL.DocuGroup.Mapper;

public class DocumentMappingProfile : Profile
{
    public DocumentMappingProfile()
    {
        CreateMap<DocumentDto, GroupDocument>();
        CreateMap<GroupDocument, DocumentDto>();
    }
}