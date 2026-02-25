using System.ComponentModel;

namespace Domain.DocuGroup;

public class GroupDocument
{
    public Guid Id { get; set; }
    public string Title { get; set; } = String.Empty;
    public DateTimeOffset LastPublishedAtUtc { get; set; }
    public string LastPublishedByUserId { get; set; } = String.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public string CreatedByUserId { get; set; } = String.Empty;
    
    // TODO Structural components (lock/edit per component via Redis using ComponentId).
    public ICollection<DocumentComponent> Components { get; set; } = new List<DocumentComponent>();
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<PublishEvent> Events { get; set; } = new List<PublishEvent>();

    public GroupDocument(Guid id, DateTimeOffset lastPublishedAtUtc, DateTimeOffset createdAtUtc)
    {
        Id = id;
        LastPublishedAtUtc = lastPublishedAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public GroupDocument()
    {
    }

    public void Publish(string newTitle, string newSnapshotJson, DateTimeOffset publishedAtUtc, string publishedByUserId)
    {
        Title = newTitle;
        LastPublishedAtUtc = publishedAtUtc;
        LastPublishedByUserId = publishedByUserId;
    }
    
    
    
}