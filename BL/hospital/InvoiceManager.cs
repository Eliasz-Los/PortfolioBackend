using AutoMapper;
using BL.hospital.dto;
using DAL.Repository.hospital;
using Domain.hospital;
using QuestPDF.Fluent;

namespace BL.hospital;

public class InvoiceManager : IInvoiceManager
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public InvoiceManager(IInvoiceRepository invoiceRepository, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }
    
    public byte[] GenerateInvoicePdf(Invoice invoice)
    {
        var document = new InvoicePdfDocument(invoice);
        return document.GeneratePdf();
    }

    public async Task<Invoice> GetById(Guid invoiceId)
    {
        return await _invoiceRepository.ReadById(invoiceId);
    }
    
    public async Task<IEnumerable<InvoiceDto>> GetAllByPatientId(Guid patientId)
    {
        var invoices =  await _invoiceRepository.ReadAllByPatientId(patientId);
        return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
    }
    
    public async Task<Invoice> Add(Invoice invoice)
    {
        return await _invoiceRepository.Create(invoice);
    }
}