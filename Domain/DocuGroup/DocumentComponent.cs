using Domain.DocuGroup.types;

namespace Domain.DocuGroup;

public class DocumentComponent
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string LastPublishedContentJson { get; set; } = String.Empty;
    public ComponentType ComponentType { get; set; }    
    public Guid GroupDocumentId { get; set; }
    public GroupDocument GroupDocument { get; set; }
    
    public DocumentComponent()
    {
    }

    public DocumentComponent(Guid id, int order, string lastPublishedContentJson, Guid groupDocumentId, ComponentType componentType)
    {
        Id = id;
        Order = order;
        LastPublishedContentJson = lastPublishedContentJson;
        GroupDocumentId = groupDocumentId;
        ComponentType = componentType;
    }
    
    public void Reorder(int newSortOrder)
    {
        if (newSortOrder < 0) throw new ArgumentOutOfRangeException(nameof(newSortOrder), "SortOrder must be >= 0.");
        Order = newSortOrder;
    }

    public void SetLastPublishedContent(string contentJson)
    {
        if (string.IsNullOrWhiteSpace(contentJson)) throw new ArgumentException("ContentJson cannot be empty.", nameof(contentJson));
        LastPublishedContentJson = contentJson;
    }
}