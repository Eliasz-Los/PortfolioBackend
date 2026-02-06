using Domain.DocuGroup.types;

namespace BL.DocuGroup.Dto.Component;

public class ChangeTypeComponentDto
{
    public Guid Id { get; set; }
    public Guid GroupDocumentId { get; set; }
    public ComponentType Type { get; set; }
    public bool clearLastPublishedContent = true;
}