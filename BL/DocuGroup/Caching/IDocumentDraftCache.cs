namespace BL.DocuGroup.Caching;

public interface IDocumentDraftCache
{
    Task<string?> GetDraftSnapshotJson(Guid documentId);
    Task SetDraftSnapshotJson(Guid documentId, string draftSnapshotJson, TimeSpan ttl);
    Task RemoveDraft(Guid documentId);
}