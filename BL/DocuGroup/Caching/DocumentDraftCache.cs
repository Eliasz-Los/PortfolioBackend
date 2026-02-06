using Microsoft.Extensions.Caching.Distributed;

namespace BL.DocuGroup.Caching;

/// <summary>
/// Stores and retrieves a draft snapshot JSON per document in Redis.
/// Keep it simple: one redis key per document.
/// </summary>
public class DocumentDraftCache : IDocumentDraftStore
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