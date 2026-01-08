using Domain.hospital;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BL.hospital;

public class InvoicePdfDocument : IDocument
{
    private readonly Invoice _invoice;
    
    public InvoicePdfDocument(Invoice invoice)
    {
        _invoice = invoice;
    }

    public DocumentMetadata GetMetadata()
    {
       return DocumentMetadata.Default;
    }
    
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
            });
        });
    }


    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("Hospital Invoice").FontSize(20).Bold();
                col.Item().Text($"Invoice Number: {_invoice.InvoiceNumber}");
                col.Item().Text($"Date: {_invoice.InvoiceDate}");
            });
            row.ConstantItem(120).AlignRight().Text(text =>
            {
                text.Span("Total Amount: ").Bold();
                text.Span($"{_invoice.Amount:C}").FontSize(16).Bold().FontColor(Colors.Blue.Medium);
            });
        });
    }


    private void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(10);
            
            col.Item().Text($"{_invoice.Title}").Bold();
            col.Item().Text($"{_invoice.Description}");
            col.Item().LineHorizontal(1);
            col.Item().Text($"Due Date: {_invoice.DueDate}");
            col.Item().Text($"Status: {(_invoice.IsPaid ? "Paid" : "Unpaid")}");
        });
    }
}