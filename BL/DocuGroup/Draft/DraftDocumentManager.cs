using System.Text.Json;
using AutoMapper;
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
    private IMapper _mapper;

    public DraftDocumentManager(IDraftSnapshotService snapshotService, IDocumentDraftCache draftCache, IDocumentRepository documentRepository, IMapper mapper)
    {
        _snapshotService = snapshotService;
        _draftCache = draftCache;
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DraftDocument> GetDraftDocumentWithComponentsById(Guid documentId)
    {
        var document = await _documentRepository.ReadDocumentWithComponentsById(documentId);
        
         var draft =  await _snapshotService.GetOrCreate(documentId);
         
         if (draft != null)
         {
             // Merge the draft snapshot with the published document structure, but not saved to the DB yet
             document.Title = draft.Title;
             foreach (var component in document.Components)
             {
                 var draftComponent = draft.Components.FirstOrDefault(c => c.Id == component.Id);
                 if (draftComponent != null)
                 {
                     component.Order = draftComponent.Order;
                     component.LastPublishedContentJson = draftComponent.LastPublishedContentJson;
                     component.ComponentType = draftComponent.ComponentType;
                 }
             }
         }

         return draft;
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