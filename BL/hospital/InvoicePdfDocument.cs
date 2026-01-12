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
            page.Content().PaddingTop(25).Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }


    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Spacing(5);
                col.Item().Text($"{_invoice.Title}").FontSize(20).Bold();
                col.Item().Text($"Invoice Number: {_invoice.InvoiceNumber}");
                col.Item().Text($"Date: {_invoice.InvoiceDate}");
                col.Item().Text($"Due Date: {_invoice.DueDate}");
            });
            row.ConstantItem(220).Column(col =>
            {
                col.Spacing(5);
                col.Item().Text("Addressed to: ").Bold();
                col.Item().Text($"{_invoice.Patient.FullName.FirstName} {_invoice.Patient.FullName.LastName}")
                    .FontSize(16).Bold().FontColor(Colors.Blue.Medium);
                col.Item().Text($"{_invoice.Patient.Location.StreetName} {_invoice.Patient.Location.StreetNumber}").FontSize(12);
                col.Item().Text($"{_invoice.Patient.Location.City} {_invoice.Patient.Location.PostalCode}").FontSize(12);
                col.Item().Text($"{_invoice.Patient.Location.Country}").FontSize(12);
            });
        });
    }
    
    private void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(20);

            // --- Table ---
            col.Item().Table(table =>
            {

                // Our columns in this case only 2 for now Description & Amount
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); 
                    columns.RelativeColumn(1); 
                });

                // Top table header
                table.Header(header =>
                {
                    header.Cell().Text("Description").Bold();
                    header.Cell().AlignRight().Text("Amount").Bold();

                    header.Cell().ColumnSpan(2)
                        .PaddingTop(5)
                        .LineHorizontal(1);
                });

                // Rows of our table
                table.Cell().Text(_invoice.Description);
                table.Cell().AlignRight().Text(_invoice.Amount.ToString("C"));
            });

            // Total amount
            col.Item().AlignRight().Column(total =>
            {
                total.Item().LineHorizontal(1);
                total.Item().PaddingTop(5).Text(text =>
                {
                    text.Span("Total: ").Bold();
                    text.Span(_invoice.Amount.ToString("C")).Bold();
                });
            });
        });
    }
    
    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("Page ");
            x.CurrentPageNumber();
        });
    }

}