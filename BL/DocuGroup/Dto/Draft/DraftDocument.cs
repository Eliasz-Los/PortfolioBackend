namespace BL.DocuGroup.Dto.Draft;

public sealed class DraftDocument
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<DraftComponent> Components { get; set; } = new List<DraftComponent>();
}