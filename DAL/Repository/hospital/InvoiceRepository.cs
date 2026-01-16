using DAL.EntityFramework;
using Domain.hospital;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.hospital;

public class InvoiceRepository : IInvoiceRepository
{
    private PortfolioDbContext _dbContext;

    public InvoiceRepository(PortfolioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Invoice?> ReadInvoiceWithPatientByInvoiceId(Guid invoiceId)
    {
        return await _dbContext.Invoices
            .Include(i => i.Patient)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);
    }

    public async Task<IEnumerable<Invoice>> ReadAllByPatientId(Guid patientId)
    {
        return await _dbContext.Invoices.Where(invoice => invoice.Patient.Id == patientId).ToListAsync();
    }

    public async Task<Invoice> Create(Invoice invoice)
    {
       
        _dbContext.Invoices.Add(invoice);
        _dbContext.SaveChanges();
        return await Task.FromResult(invoice);
    }
}