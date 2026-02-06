using BL.DocuGroup.Dto.Component;
using Domain.DocuGroup;

namespace BL.DocuGroup;

public interface IComponentManager
{
    Task<DocumentComponent> GetComponentByDocumentIdAndComponentId(Guid documentId, Guid componentId);
    Task AddComponentForDocumentByDocumentId( AddComponentDto componentDto);
    Task ChangeContent(ChangeContentComponentDto changeContentDto);
    Task ReorderComponent(ReorderComponentDto reorderComponentDto);
    Task ChangeType(ChangeTypeComponentDto changeTypeComponentDto);
    Task RemoveComponent(Guid documentId, Guid componentId);
}