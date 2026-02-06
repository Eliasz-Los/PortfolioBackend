namespace BL.DocuGroup.Dto.Component;

public class ChangeContentComponentDto
{
    public Guid Id { get; set; }
    public string? LastPublishedContentJson { get;  set; }
    public Guid GroupDocumentId { get; set; }
}