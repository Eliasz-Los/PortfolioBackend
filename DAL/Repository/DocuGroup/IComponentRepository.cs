using Domain.DocuGroup;
using Domain.DocuGroup.types;

namespace DAL.Repository.DocuGroup;

public interface IComponentRepository
{
    Task<DocumentComponent> ReadComponentByDocumentIdAndComponentId(Guid documentId, Guid componentId);
    Task<IEnumerable<DocumentComponent>> ReadAllComponentsByDocumentId(Guid documentId);
    Task CreateComponentForDocumentByDocumentId(DocumentComponent component);
    Task UpdateContent(Guid documentId,Guid componentId ,string? lastPublishedContentJson);
    Task ReorderComponent(Guid documentId, Guid componentId, int newOrder);
    Task UpdateType(Guid documentId, Guid componentId, ComponentType newType, bool clearLastPublishedContent = true);
    Task DeleteComponent(Guid documentId, Guid componentId);
}