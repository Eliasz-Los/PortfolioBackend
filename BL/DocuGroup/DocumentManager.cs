using DAL.Repository.DocuGroup;
using Domain.DocuGroup;

namespace BL.DocuGroup;

public class DocumentManager : IDocumentManager
{
    private readonly IDocumentRepository _repository;

    public DocumentManager(IDocumentRepository repository)
    {
        _repository = repository;
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
        await _repository.CreateDocument(document);
    }

    public async Task DeleteDocument(Guid documentId)
    {
        await _repository.RemoveDocument(documentId);
    }
}