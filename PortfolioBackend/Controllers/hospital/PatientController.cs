using BL.hospital;
using BL.hospital.dto;
using BL.hospital.invoice;
using Domain.hospital;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/hospital/patients")]
public class PatientController : ControllerBase
{
    private readonly IBaseManager<Patient, PatientDto, AddPatientDto> _patientManager;
    private readonly IInvoiceManager _invoiceManager;

    public PatientController(IBaseManager<Patient, PatientDto, AddPatientDto> patientManager, IInvoiceManager invoiceManager)
    {
        _patientManager = patientManager;
        _invoiceManager = invoiceManager;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPatient(Guid id)
    {
        
        var patient = await _patientManager.GetById(id);
        
        if (patient == null)
        {
            return NotFound();
        }
        
        return Ok(patient);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPatients()
    {
        var patients = await  _patientManager.GetAll();

        if (!patients.Any())
        {
            return NotFound();
        }
        
        return Ok(patients);
    }

    [HttpPost]
    public async Task<IActionResult> AddPatient(AddPatientDto patient)
    {
         var createdPatient = await _patientManager.Add(patient);
         return CreatedAtAction(nameof(GetPatient), new { id = createdPatient.Id }, createdPatient);
    }


    [HttpDelete("{id:guid}")]
    public IActionResult DeletePatient(Guid id)
    {
        _patientManager.Remove(id);
        return NoContent();
    }

    [HttpGet("{id:guid}/invoices")]
    public async Task<IActionResult> GetPatientInvoices(Guid id)
    {
        var invoices = await _invoiceManager.GetAllByPatientId(id);
        
        if (!invoices.Any())
        {
            return NotFound();
        }
        
        return Ok(invoices);
    }
    
}