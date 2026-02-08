using BL.DocuGroup.Dto.Document;
using Domain.DocuGroup;

namespace BL.DocuGroup;

public interface IDocumentManager
{
    Task<GroupDocument?> GetDocumentWithComponentsById(Guid documentId);
    Task<IEnumerable<DocumentDto>> GetAllDocumentsByUserId(string userId);
    Task AddDocument(AddDocumentDto documentDto, string userId);
    Task DeleteDocument(Guid documentId);
    Task PublishDocument(PublishDto publishDto);
}