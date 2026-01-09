namespace Domain.hospital;

public class Invoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public decimal Amount { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateOnly DueDate { get; set; }
    public bool IsPaid { get; set; }
    public Patient Patient { get; set; }
    
    public Invoice(string invoiceNumber, DateOnly invoiceDate, decimal amount, string title, string description, DateOnly dueDate, Guid id, bool isPaid = false)
    {
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        Amount = amount;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Id = id;
        IsPaid = isPaid;
    }

    public Invoice()
    {
    }
}