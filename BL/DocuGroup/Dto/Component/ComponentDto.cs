using Domain.DocuGroup.types;

namespace BL.DocuGroup.Dto.Component;

public class ComponentDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string LastPublishedContentJson { get; set; } = string.Empty;
    public ComponentType ComponentType { get; set; }
    public Guid GroupDocumentId { get; set; }
}