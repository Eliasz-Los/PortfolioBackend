namespace BL.hospital.dto;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } 
    public DateOnly DueDate { get; set; }
    public bool IsPaid { get; set; }
}