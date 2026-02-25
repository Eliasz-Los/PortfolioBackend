using System.ComponentModel;
using System.Text.Json;
using AutoMapper;
using BL.DocuGroup.Caching;
using BL.DocuGroup.Draft;
using BL.DocuGroup.Dto.Component;
using BL.DocuGroup.Dto.Document;
using BL.DocuGroup.Dto.Draft;
using DAL.Repository.DocuGroup;
using DAL.Repository.UoW;
using Domain.DocuGroup;
using Domain.DocuGroup.types;

namespace BL.DocuGroup;

/// <summary>
/// Snapshot driven document manager.
/// All document changes are made to a draft snapshot in cache,
/// and then published to the database when the user chooses to publish.
/// This allows for a more seamless user experience,
/// as they can make multiple changes and only save once,
/// rather than having to wait for each change to be saved to the database.
/// </summary>
public class DocumentManager : IDocumentManager
{
    private readonly IDocumentRepository _repository;
    private readonly IComponentManager _componentManager;
    private readonly IUnitOfWork _uow;
    private readonly IDocumentDraftCache _draftCache;
    private readonly IMembershipManager _membershipManager;
    private readonly IMapper _mapper;
    private readonly IDraftDocumentManager _draftDocumentManager;
    

    public DocumentManager(IDocumentRepository repository, IUnitOfWork uow, IComponentManager componentManager, IDocumentDraftCache draftCache, IMembershipManager membershipManager, IMapper mapper, IDraftDocumentManager draftDocumentManager)
    {
        _repository = repository;
        _uow = uow;
        _componentManager = componentManager;
        _draftCache = draftCache;
        _membershipManager = membershipManager;
        _mapper = mapper;
        _draftDocumentManager = draftDocumentManager;
    }

    public async Task<DocumentDetailsDto> GetDocumentWithComponentsById(Guid documentId)
    {
        var document = await _repository.ReadDocumentWithComponentsById(documentId);
        return _mapper.Map<DocumentDetailsDto>(document);
    }

    public async Task<IEnumerable<DocumentDto>> GetAllDocumentsByUserId(string userId)
    {
        var documents = await _repository.ReadAllDocumentsByUserId(userId);
        return _mapper.Map<IEnumerable<DocumentDto>>(documents);
    }

    public async Task AddDocument(AddDocumentDto document, string userId)
    {
        await _uow.BeginTransaction();
        try
        {
            GroupDocument newDocument = new GroupDocument()
            {
                Id = Guid.NewGuid(),
                Title = document.Title,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                CreatedByUserId = userId
            };
           
            await _repository.CreateDocument(newDocument);
            
            await _membershipManager.AddMembership(
                new Membership(id: Guid.NewGuid(), 
                groupDocumentId: newDocument.Id, 
                role: DocumentRole.Owner, 
                userId: userId, 
                lastSeenAtUtc: DateTimeOffset.UtcNow));
            await _uow.Commit();
            
            var snapshot = new DraftDocument
            {
                Id = newDocument.Id,
                Title = newDocument.Title,
                Components = new List<DraftComponent>
                {
                    new DraftComponent
                    {
                        Id =  Guid.NewGuid(),
                        Order = 1,
                        ComponentType = ComponentType.Title,
                        LastPublishedContentJson = "Welcome to your new document!"
                    },
                    new DraftComponent
                    {
                        Id =  Guid.NewGuid(),
                        Order = 2,
                        ComponentType = ComponentType.Paragraph,
                        LastPublishedContentJson = "Here is a sample paragraph. You can edit this content."
                    }
                }
            };
            
            await _draftDocumentManager.SaveDraftSnapshot(snapshot);
            
        }
        catch
        {
            await _uow.Rollback();
            throw;
        }
       
    }

    public async Task DeleteDocument(Guid documentId)
    {
        
        await _uow.BeginTransaction();
        try
        {
            await _repository.DeleteDocument(documentId);
            await _uow.Commit();
        }
        catch
        {
            await _uow.Rollback();
            throw;
        }
    }
    
    public async Task PublishDocument(PublishDto publishDto)
    {
        await _uow.BeginTransaction();
        try
        {
            var draft = await _draftDocumentManager.GetDraftDocumentWithComponentsById(publishDto.Id);
            
            var desiredComps= draft.Components
                .OrderBy(c => c.Order)
                .Select(c => new DocumentComponent
                {
                    Id = c.Id,
                    Order = c.Order,
                    ComponentType = c.ComponentType,
                    LastPublishedContentJson = c.LastPublishedContentJson
                })
                .ToList();
            
            await _componentManager.SyncComponentsByDocumentId(publishDto.Id, desiredComps);
            await _uow.Commit(); // Here is it actually saved to the database and becomes the new published version of the document
            await _draftCache.RemoveDraft(publishDto.Id);
        }
        catch
        {
            await _uow.Rollback();
            throw;
        }
    }
}