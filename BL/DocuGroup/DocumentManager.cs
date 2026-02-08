using AutoMapper;
using BL.DocuGroup.Caching;
using BL.DocuGroup.Dto.Component;
using BL.DocuGroup.Dto.Document;
using BL.DocuGroup.Dto.Draft;
using DAL.Repository.DocuGroup;
using DAL.Repository.UoW;
using Domain.DocuGroup;
using Domain.DocuGroup.types;

namespace BL.DocuGroup;

/// <summary>
/// Document manager \- serve draft from redis cache if it exists, otherwise published.
/// Publish copies draft \-> DB and clears draft.
/// </summary>
public class DocumentManager : IDocumentManager, IDraftDocumentManager
{
    private readonly IDocumentRepository _repository;
    private readonly IComponentManager _componentManager;
    private readonly IUnitOfWork _uow;
    private readonly IDocumentDraftStore _draftStore;
    private readonly IMembershipManager _membershipManager;
    private readonly IMapper _mapper;

    //TODO: more dto's
    public DocumentManager(IDocumentRepository repository, IUnitOfWork uow, IComponentManager componentManager, IDocumentDraftStore draftStore, IMembershipManager membershipManager, IMapper mapper)
    {
        _repository = repository;
        _uow = uow;
        _componentManager = componentManager;
        _draftStore = draftStore;
        _membershipManager = membershipManager;
        _mapper = mapper;
    }

    public async Task<GroupDocument?> GetDocumentWithComponentsById(Guid documentId)
    {
        return await _repository.ReadDocumentWithComponentsById(documentId);
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
         
            await _componentManager.AddComponentForDocumentByDocumentId( new AddComponentDto
            { 
               LastPublishedContentJson = "Welcome to your new document!",
               GroupDocumentId = newDocument.Id,
               ComponentType = ComponentType.Title
            });
            await _componentManager.AddComponentForDocumentByDocumentId( new AddComponentDto
            {
                LastPublishedContentJson = "Here is a sample paragraph. You can edit this content.",
                GroupDocumentId = newDocument.Id,
                ComponentType = ComponentType.Paragraph
            });
            await _uow.Commit();
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
            var draft = await _draftStore.GetDraftSnapshotJson(publishDto.Id);
            if (string.IsNullOrWhiteSpace(draft))
            {
                throw new InvalidOperationException($"No draft exists to publish for this documentId: {publishDto.Id}.");
            }

            var document = await _repository.ReadDocumentById(publishDto.Id);
            document.Publish(publishDto.Title, draft, DateTimeOffset.UtcNow, publishDto.publishedByUserId);
            await _uow.Commit();
            
            await _draftStore.RemoveDraft(publishDto.Id);
        }
        catch
        {
            await _uow.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Get document with components, serving draft from cache if it exists.
    /// </summary>
    public async Task<GroupDocument> GetDraftDocumentWithComponentsById(Guid documentId)
    {
        var document = await _repository.ReadDocumentWithComponentsById(documentId);

        var draft = await _draftStore.GetDraftSnapshotJson(documentId);

        if (!string.IsNullOrWhiteSpace(draft))
        {
            document.Publish(document.Title, draft, document.LastPublishedAtUtc, document.LastPublishedByUserId);
        }
        return document;
    }

    
    //TODO : toevoegen bij components ipv ineens op te slaan
    /// <summary>
    /// Call this from component edits/reorder/type changes instead of saving to DB.
    /// Store the whole document snapshot as JSON in Redis.
    /// This way it's easy faster to edit multiple components before publishing.
    /// </summary>
    public async Task SaveDraftSnapshot(DraftDto draftDto)
    {
        await _draftStore.SetDraftSnapshotJson(draftDto.Id, draftDto.SnapshotJson, TimeSpan.FromDays(30));
    }
}