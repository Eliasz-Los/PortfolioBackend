using BL.hospital;
using BL.hospital.dto;
using Domain.hospital;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/hospital/doctors")]
public class DoctorController : ControllerBase
{
  private readonly IBaseManager<Doctor, DoctorDto, AddDoctorDto> _doctorManager;

  public DoctorController(IBaseManager<Doctor, DoctorDto, AddDoctorDto> doctorManager)
  {
    _doctorManager = doctorManager;
  }


  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetDoctor(Guid id)
  {
    
    var doctor = await _doctorManager.GetById(id);
    
    if (doctor == null)
    {
      return NotFound();
    }

    return Ok(doctor);
  }


  [HttpGet]
  public async Task<IActionResult> GetDoctors()
  {
     var doctors = await  _doctorManager.GetAll();
      if (!doctors.Any())
      {
        return NotFound();
      }
      return Ok(doctors);
  }

  [HttpPost]
  public async Task<IActionResult> AddDoctor(AddDoctorDto doctor)
  {
    var createdDoctor = await _doctorManager.Add(doctor);
    return CreatedAtAction(nameof(GetDoctor), new { id = createdDoctor.Id }, createdDoctor);
  }

  [HttpDelete("{id:guid}")]
  public IActionResult DeleteDoctor(Guid id)
  {
    _doctorManager.Remove(id);
    return NoContent();
  }
  
}