using BL.hospital;
using BL.hospital.dto;
using Domain.hospital;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/hospital/")]
public class PatientController : Controller
{
    private readonly IBaseManager<Patient> _baseManager;

    public PatientController(IBaseManager<Patient> baseManager)
    {
        _baseManager = baseManager;
    }

    [HttpGet("patient/{id}")]
    public async Task<IActionResult> GetPatient(Guid id)
    {
        var patient = await _baseManager.GetById(id);
        if (patient == null)
        {
            return Empty;
        }
        return Ok(patient);
    }
    
    [HttpGet("patients")]
    public async Task<IActionResult> GetPatients()
    {
        var patients = await  _baseManager.GetAll();

        if (!patients.Any())
        {
            return NotFound();
        }
        
        return Ok(patients);
    }

    [HttpPost("patient")]
    public IActionResult AddPatient(Patient patient)
    {
         _baseManager.Add(patient);
         return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
    }


    [HttpDelete("patient/{id}")]
    public IActionResult DeletePatient(Guid id)
    {
        _baseManager.Remove(id);
        return NoContent();
    }
    
}