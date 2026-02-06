namespace BL.DocuGroup.Dto.Document;

public class PublishDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string publishedByUserId { get; set; } = string.Empty;
    
}