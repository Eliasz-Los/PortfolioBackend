using BL.DocuGroup.Dto.Component;
using Domain.DocuGroup;

namespace BL.DocuGroup;

public interface IComponentManager
{
    Task<DocumentComponent> GetComponentByDocumentIdAndComponentId(Guid documentId, Guid componentId);
    Task AddComponentForDocumentByDocumentId( AddComponentDto componentDto);
    Task RemoveComponent(Guid documentId, Guid componentId);
}