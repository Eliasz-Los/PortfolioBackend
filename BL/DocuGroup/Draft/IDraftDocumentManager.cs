using BL.DocuGroup.Dto.Draft;
using Domain.DocuGroup;

namespace BL.DocuGroup.Draft;

public interface IDraftDocumentManager
{
    Task<DraftDocument> GetDraftDocumentWithComponentsById(Guid documentId);
    Task SaveDraftSnapshot(DraftDocument draftDocument);
    Task Remove(Guid documentId);
}