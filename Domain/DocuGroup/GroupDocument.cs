using System.ComponentModel;

namespace Domain.DocuGroup;

public class GroupDocument
{
    public Guid Id { get; set; }
    public string Title { get; set; } = String.Empty;
    // Latest published snapshot in DB (single source of truth for published state)
    public string SnapshotJson { get; private set; } = "{}";
    public DateTimeOffset LastPublishedAtUtc { get; set; }
    public string LastPublishedByUserId { get; set; } = String.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public string CreatedByUserId { get; set; } = String.Empty;
    
    // Structural components (lock/edit per component via Redis using ComponentId).
    public ICollection<DocumentComponent> Components { get; set; } = new List<DocumentComponent>();
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<PublishEvent> Events { get; set; } = new List<PublishEvent>();

    public GroupDocument(Guid id, string snapshotJson, DateTimeOffset lastPublishedAtUtc, DateTimeOffset createdAtUtc)
    {
        Id = id;
        SnapshotJson = snapshotJson;
        LastPublishedAtUtc = lastPublishedAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public GroupDocument()
    {
    }

    public void Publish(string newTitle, string newSnapshotJson, DateTimeOffset publishedAtUtc, string publishedByUserId)
    {
        Title = newTitle;
        SnapshotJson = newSnapshotJson;
        LastPublishedAtUtc = publishedAtUtc;
        LastPublishedByUserId = publishedByUserId;
    }
    
    
    
}