using Domain.hospital;

namespace DAL.Repository.hospital;

public interface IInvoiceRepository
{
    Task<Invoice> ReadById(Guid invoiceId);
    Task<IEnumerable<Invoice>> ReadAllByPatientId(Guid patientId);
    Task<Invoice> Create(Invoice invoice);
}