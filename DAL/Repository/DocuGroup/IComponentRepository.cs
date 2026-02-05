using Domain.DocuGroup;
using Domain.DocuGroup.types;

namespace DAL.Repository.DocuGroup;

public interface IComponentRepository
{
    Task<DocumentComponent?> ReadComponentByDocumentIdAndComponentId(Guid documentId, Guid componentId);

    Task CreateComponentForDocumentByDocumentId(Guid documentId, DocumentComponent component);

    Task ReorderComponent(Guid documentId, Guid componentId, int newOrder);
    Task ChangeType(Guid documentId, Guid componentId, ComponentType newType, bool clearLastPublishedContent = true);

}