using BL.hospital.dto;
using Domain.hospital;

namespace BL.hospital.invoice;

public interface IInvoiceManager
{
    byte[] GenerateInvoicePdf(Invoice invoice);

    Task<Invoice?> GetInvoiceWithPatientByInvoiceId(Guid invoiceId);
    Task<IEnumerable<InvoiceDto>> GetAllByPatientId(Guid patientId);
    Task<Invoice?> Add(Invoice? invoice);
}