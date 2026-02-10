using BL.DocuGroup.Dto.Draft;

namespace BL.DocuGroup.Draft;

public interface IDraftSnapshotService
{
    Task<DraftDocument> GetOrCreate(Guid documentId);
    Task Save(Guid documentId, DraftDocument document, TimeSpan? ttl = null);
    Task RemoveDraftDocument(Guid documentId);
    Task RemoveComponentFromDraftDocument(Guid documentId, Guid componentId);
}