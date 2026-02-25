namespace BL.DocuGroup.Dto;

public class DocEvent
{
    public string Type { get; set; } = string.Empty;
    public Guid DocumentId { get; set; }
    public object Payload { get; set; } = new object();
    public long Version { get; set; }
}