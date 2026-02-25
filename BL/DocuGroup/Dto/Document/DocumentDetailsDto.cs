using BL.DocuGroup.Dto.Component;

namespace BL.DocuGroup.Dto.Document;

public class DocumentDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTimeOffset LastPublishedAtUtc { get; set; }
    public string LastPublishedByUserId { get; set; } = string.Empty;

    public List<ComponentDto> Components { get; set; } = new();
}