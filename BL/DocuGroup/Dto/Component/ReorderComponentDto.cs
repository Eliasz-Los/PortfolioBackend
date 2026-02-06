namespace BL.DocuGroup.Dto.Component;

public class ReorderComponentDto
{
    public Guid Id { get; set; }
    public Guid GroupDocumentId { get; set; }
    public int NewOrder { get; set; }
}