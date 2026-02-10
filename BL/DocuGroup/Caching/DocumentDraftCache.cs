using Microsoft.Extensions.Caching.Distributed;

namespace BL.DocuGroup.Caching;

/// <summary>
/// Im using Snapshot-driven publishing pattern where the current state of a document is stored as a snapshot in a cache.
/// When a user wants to publish the document,
/// the system retrieves the snapshot from the cache and uses it to create a published version of the document.
/// This approach allows for quick retrieval of the document's state without needing to query the database,
/// improving performance and scalability.
/// </summary>
public class DocumentDraftCache : IDocumentDraftCache
{
    private readonly IDistributedCache _cache;

    public DocumentDraftCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    private static string Key(Guid documentId)
    {
        return $"DocuGroup:Draft:{documentId}";
    }

    public async Task<string?> GetDraftSnapshotJson(Guid documentId)
    {
        return await _cache.GetStringAsync(Key(documentId));
    }

    public async Task SetDraftSnapshotJson(Guid documentId, string draftSnapshotJson, TimeSpan ttl )
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        await _cache.SetStringAsync(Key(documentId), draftSnapshotJson, options);
    }

    public async Task RemoveDraft(Guid documentId)
    {
        await _cache.RemoveAsync(Key(documentId));
    }
    
}