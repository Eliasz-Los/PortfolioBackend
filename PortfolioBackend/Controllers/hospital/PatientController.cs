using BL.hospital;
using BL.hospital.dto;
using Domain.hospital;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/hospital/patients")]
public class PatientController : ControllerBase
{
    private readonly IBaseManager<Patient, PatientDto, AddPatientDto> _patientManager;

    public PatientController(IBaseManager<Patient, PatientDto, AddPatientDto> patientManager)
    {
        _patientManager = patientManager;
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
    
}