using Domain.DocuGroup.types;

namespace BL.DocuGroup.Dto.Component;

public class AddComponentDto
{
    public string? LastPublishedContentJson { get;  set; }
    public ComponentType ComponentType { get; set; }   
    public Guid GroupDocumentId { get; set; }
}