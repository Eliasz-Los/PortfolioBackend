using BL.DocuGroup.Dto.Draft;
using Domain.DocuGroup;

namespace BL.DocuGroup;

public interface IDraftDocumentManager
{
    Task<GroupDocument> GetDraftDocumentWithComponentsById(Guid documentId);
    Task SaveDraftSnapshot(DraftDto draftDto);
}