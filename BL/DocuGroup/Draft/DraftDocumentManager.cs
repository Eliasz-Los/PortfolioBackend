using System.Text.Json;
using BL.DocuGroup.Caching;
using BL.DocuGroup.Dto.Draft;
using DAL.Repository.DocuGroup;
using Domain.DocuGroup;

namespace BL.DocuGroup.Draft;

public sealed class DraftDocumentManager :  IDraftDocumentManager
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IDocumentRepository _documentRepository;
    private readonly IDraftSnapshotService _snapshotService;
    private readonly IDocumentDraftCache _draftCache;

    public DraftDocumentManager(IDraftSnapshotService snapshotService, IDocumentDraftCache draftCache, IDocumentRepository documentRepository)
    {
        _snapshotService = snapshotService;
        _draftCache = draftCache;
        _documentRepository = documentRepository;
    }

    public async Task<GroupDocument> GetDraftDocumentWithComponentsById(Guid documentId)
    {
        var document = await _documentRepository.ReadDocumentWithComponentsById(documentId);
         await _snapshotService.GetOrCreate(documentId);

         var draftJson = await _draftCache.GetDraftSnapshotJson(documentId);

         if (!string.IsNullOrWhiteSpace(draftJson))
         {
             // We have a draft, so we overwrite the document's title and components with the draft values
             // but not saved until actual publishing,
             // this is just for the user to see their draft changes when they open the document again
             document.Publish(document.Title, draftJson, document.LastPublishedAtUtc, document.LastPublishedByUserId);
         }
        return document;
    }

    public async Task SaveDraftSnapshot(DraftDocument draftDocument)
    {
        await _snapshotService.Save(draftDocument.Id, draftDocument, TimeSpan.FromDays(30));
    }

    public Task Remove(Guid documentId)
    {
        return _snapshotService.RemoveDraftDocument(documentId);
    }
}