using BL.hospital;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital
{
    [Route("api/hospital/invoices")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {

        private readonly IInvoiceManager _invoiceManager;

        public InvoiceController(IInvoiceManager invoiceManager)
        {
            _invoiceManager = invoiceManager;
        }

        [HttpGet("{id:guid}/pdf")]
        public async Task<ActionResult> GetInvoicePdf(Guid id)
        {
            var invoice = await _invoiceManager.GetById(id);
            
            if (invoice == null)
            {
                return NotFound();
            }
            
            var pdfBytes = _invoiceManager.GenerateInvoicePdf(invoice);
            return File(pdfBytes, "application/pdf", $"Invoice_{invoice.InvoiceNumber}.pdf");
        }
    }
}
