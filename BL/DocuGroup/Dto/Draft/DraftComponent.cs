using Domain.DocuGroup.types;

namespace BL.DocuGroup.Dto.Draft;

public sealed class DraftComponent
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public ComponentType ComponentType { get; set; }
    public string? LastPublishedContentJson { get;  set; }
}