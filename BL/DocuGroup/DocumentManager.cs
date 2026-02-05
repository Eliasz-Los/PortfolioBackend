using DAL.Repository.DocuGroup;
using DAL.Repository.UoW;
using Domain.DocuGroup;
using Domain.DocuGroup.types;

namespace BL.DocuGroup;

public class DocumentManager : IDocumentManager
{
    private readonly IDocumentRepository _repository;
    private readonly IComponentRepository _componentRepository;
    private readonly UnitOfWork _uow;

    public DocumentManager(IDocumentRepository repository, UnitOfWork uow, IComponentRepository componentRepository)
    {
        _repository = repository;
        _uow = uow;
        _componentRepository = componentRepository;
    }

    public async Task<GroupDocument?> GetDocumentWithComponentsById(Guid documentId)
    {
        return await _repository.ReadDocumentWithComponentsById(documentId);
    }

    public async Task<IEnumerable<GroupDocument>> GetAllDocumentsByUserId(string userId)
    {
        return await _repository.ReadAllDocumentsByUserId(userId);
    }

    public async Task AddDocument(GroupDocument document)
    {
        await _uow.BeginTransaction();
        try
        {
            await _repository.CreateDocument(document);
         
            await _componentRepository.CreateComponentForDocumentByDocumentId(document.Id, new DocumentComponent
            { 
                Id = Guid.NewGuid(),
               Order = 1,
               LastPublishedContentJson = "Welcome to your new document!",
               GroupDocumentId = document.Id,
               ComponentType = ComponentType.Title
            });
            await _componentRepository.CreateComponentForDocumentByDocumentId(document.Id, new DocumentComponent
            {
                Id = Guid.NewGuid(),
                Order = 2,
                LastPublishedContentJson = "Here is a sample paragraph. You can edit this content.",
                GroupDocumentId = document.Id,
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
        await _repository.RemoveDocument(documentId);
    }
}