using BL.DocuGroup.Dto.Component;

namespace BL.DocuGroup.Draft;

public interface IDraftComponentManager
{
    Task AddComponent(AddComponentDto dto);
    Task ChangeContent(ChangeContentComponentDto dto);
    Task Reorder(ReorderComponentDto dto);
    Task ChangeType(ChangeTypeComponentDto dto);
    Task Remove(Guid documentId, Guid componentId);
}