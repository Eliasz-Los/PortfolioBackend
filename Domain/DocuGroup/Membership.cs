using Domain.DocuGroup.types;

namespace Domain.DocuGroup;

public class Membership
{
    public Guid Id { get; set; }

    public Guid GroupDocumentId { get; set; }
    public GroupDocument GroupDocument { get; set; }
    //Keycloack user id (jwt sub claim)
    public string UserId { get; set; } = string.Empty;
    public DocumentRole Role { get; private set; } = DocumentRole.Editor;
    public DateTimeOffset JoinedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastSeenAtUtc { get; set;  }
    
    public Membership() {}

    public Membership(Guid id, Guid groupDocumentId, DocumentRole role, string userId, DateTimeOffset? lastSeenAtUtc)
    {
        Id = id;
        GroupDocumentId = groupDocumentId;
        UserId = userId;
        Role = role;
        LastSeenAtUtc = lastSeenAtUtc;
    }
    
    public void ChangeRole(DocumentRole newRole)
    {
        Role = newRole;
    }
}