using System.Net.Mail;
using System.Reflection;
using Domain.hospital;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BL.hospital.invoice;

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

                col.Item().AlignLeft()
                    .Image(GetLogoPath()).FitWidth();                
                // Invoice title
                col.Item().PaddingTop(5).Text($"{_invoice.Title}")
                    .FontSize(20)
                    .Bold();

                // Invoice metadata
                col.Item().Text($"Invoice Number: {_invoice.InvoiceNumber}");
                col.Item().Text($"Date: {_invoice.InvoiceDate}");
                col.Item().Text($"Due Date: {_invoice.DueDate}");
            });

            row.ConstantItem(220).Column(col =>
            {
                col.Spacing(5);
                col.Item().Text("Bill To:").Bold();
                col.Item().Text($"{_invoice.Patient.FullName.FirstName} {_invoice.Patient.FullName.LastName}")
                    .FontSize(16).Bold().FontColor(Colors.Blue.Medium);
                col.Item().Text($"{_invoice.Patient.Location.StreetName} {_invoice.Patient.Location.StreetNumber}");
                col.Item().Text($"{_invoice.Patient.Location.PostalCode} { _invoice.Patient.Location.City}");
                col.Item().Text($"{_invoice.Patient.Location.Country}");
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
                    header.Cell().Background(Colors.Grey.Lighten2).Text("Description").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).AlignRight().Text("Amount").Bold();
                    header.Cell().ColumnSpan(2).PaddingTop(5).LineHorizontal(1);
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
            
            col.Item().Column(payment =>
            {
                payment.Spacing(5);
                payment.Item().Text("Please pay the amount to the following account:").Bold();
                payment.Item().Text("IBAN: BE12 3456 7890 1234").FontSize(12);
                payment.Item().Text("BIC: ABCDBEBB").FontSize(12);
                payment.Item().Text($"Structured message: INV-{_invoice.InvoiceNumber}").FontSize(12);
            });

        });
    }
    
    private void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text("Contact: info@myhospital.be | +32 123 456 789")
                .FontSize(9).FontColor(Colors.Grey.Darken1);
            row.ConstantItem(100).AlignRight().Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        });
    }

    private static string GetLogoPath()
    {
           return Path.Combine(AppContext.BaseDirectory,"assets" ,"logo.png"); 
    }
}