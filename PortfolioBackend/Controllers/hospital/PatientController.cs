using BL.hospital;
using BL.hospital.dto;
using Domain.hospital;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/hospital/[controller]")]
public class PatientController : Controller
{
    private readonly IPatientManager _patientManager;

    public PatientController( IPatientManager patientManager)
    {
        _patientManager = patientManager;
    }

    [HttpGet("patient/{id}")]
    public async Task<IActionResult> GetPatient(Guid id)
    {
        var patient = await _patientManager.GetPatientById(id);
        if (patient == null)
        {
            return Empty;
        }
        return Ok(patient);
    }
    
    [HttpGet("patients")]
    public async Task<IActionResult> GetPatients()
    {
        var patients = await  _patientManager.GetAllPatients();

        if (!patients.Any())
        {
            return NotFound();
        }
        
        return Ok(patients);
    }

    [HttpPost("patient")]
    public async Task<IActionResult> AddPatient(AddPatientDto patient)
    {
         var createdPatient = await _patientManager.AddPatient(patient);
         return CreatedAtAction(nameof(GetPatient), new { id = createdPatient.Id }, patient);
    }


    [HttpDelete("patient/{id}")]
    public IActionResult DeletePatient(Guid id)
    {
        _patientManager.RemovePatient(id);
        return NoContent();
    }
    
}