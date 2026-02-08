namespace BL.DocuGroup.Dto.Document;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}