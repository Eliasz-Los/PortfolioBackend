using System.Text.Json;
using BL.DocuGroup.Caching;
using BL.DocuGroup.Dto.Draft;
using DAL.Repository.DocuGroup;

namespace BL.DocuGroup.Draft;

public sealed class DraftSnapshotService : IDraftSnapshotService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentDraftCache _draftCache;

    public DraftSnapshotService(IDocumentRepository documentRepository, IDocumentDraftCache draftCache)
    {
        _documentRepository = documentRepository;
        _draftCache = draftCache;
    }

    public async Task<DraftDocument> GetOrCreate(Guid documentId)
    {
        var existingJson = await _draftCache.GetDraftSnapshotJson(documentId);
        if (!string.IsNullOrWhiteSpace(existingJson))
        {
            var existing = JsonSerializer.Deserialize<DraftDocument>(existingJson, JsonOptions);
            if (existing != null) return existing;
        }

        // No draft -> build a draft snapshot from the published DB state (structure)
        var doc = await _documentRepository.ReadDocumentWithComponentsById(documentId);
        var snapshot = new DraftDocument
        {
            Id = doc.Id,
            Title = doc.Title,
            Components = doc.Components
                .OrderBy(c => c.Order)
                .Select(c => new DraftComponent
                {
                    Id = c.Id,
                    Order = c.Order,
                    ComponentType = c.ComponentType,
                    LastPublishedContentJson = c.LastPublishedContentJson
                })
                .ToList()
        };

        await Save(documentId, snapshot);
        return snapshot;
    }

    public async Task Save(Guid documentId, DraftDocument document, TimeSpan? ttl = null)
    {
        var json = JsonSerializer.Serialize(document, JsonOptions);
        await _draftCache.SetDraftSnapshotJson(documentId, json, ttl ?? TimeSpan.FromDays(30));
    }

    public async Task RemoveDraftDocument(Guid documentId)
    {
        await _draftCache.RemoveDraft(documentId);
    }

    public async Task RemoveComponentFromDraftDocument(Guid documentId, Guid componentId)
    {
        var draft = await GetOrCreate(documentId);
        var draftComponent = draft.Components.Find(c => c.Id == componentId);
        if (draftComponent != null)
        {
            draft.Components.Remove(draftComponent);
        }
        else
        {
            throw new KeyNotFoundException($"Component with id {componentId} not found in draft for document {documentId}.");
        }
       
        await Save(documentId, draft);
    }
}
