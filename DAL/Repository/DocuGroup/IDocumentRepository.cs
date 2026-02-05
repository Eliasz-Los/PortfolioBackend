using Domain.DocuGroup;

namespace DAL.Repository.DocuGroup;

public interface IDocumentRepository
{
    Task<GroupDocument?> ReadDocumentById(Guid documentId);
    Task<GroupDocument?> ReadDocumentWithComponentsById(Guid documentId);
    Task<IEnumerable<GroupDocument>> ReadAllDocumentsByUserId(string userId);
    Task CreateDocument(GroupDocument document);
    Task RemoveDocument(Guid documentId);
}