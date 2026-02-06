using BL.DocuGroup.Dto.Document;
using Domain.DocuGroup;

namespace BL.DocuGroup;

public interface IDocumentManager
{
    Task<GroupDocument?> GetDocumentWithComponentsById(Guid documentId);
    Task<IEnumerable<GroupDocument>> GetAllDocumentsByUserId(string userId);
    Task AddDocument(GroupDocument document);
    Task DeleteDocument(Guid documentId);
    Task PublishDocument(PublishDto publishDto);
}