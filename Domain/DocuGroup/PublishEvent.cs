namespace Domain.DocuGroup;

public class PublishEvent
{
    public Guid Id { get; set; }
    public Guid GroupDocumentId { get; set; }
    public GroupDocument GroupDocument { get; set; }
    public int PublishNumber { get; set; }
    public DateTimeOffset PublishedAtUtc { get; set; }
    public string PublishedByUserId { get; set; } = String.Empty;
    //This way we store the fullsnapshot for history tracking
    public string SnapshotJson { get; set; } = "{}";
    
    public PublishEvent() { }

    public PublishEvent(Guid id, Guid groupDocumentId, int publishNumber, DateTimeOffset publishedAtUtc, string snapshotJson)
    {
        Id = id;
        GroupDocumentId = groupDocumentId;
        PublishNumber = publishNumber;
        PublishedAtUtc = publishedAtUtc;
        SnapshotJson = snapshotJson;
    }
}