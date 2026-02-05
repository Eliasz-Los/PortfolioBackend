using BL.hospital;
using BL.hospital.dto;
using Domain.hospital;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/Hospital/doctors")]
public class DoctorController : ControllerBase
{
  private readonly IBaseManager<Doctor, DoctorDto, AddDoctorDto> _baseManager;
  private readonly IDoctorManager _doctorManager;

  public DoctorController(IBaseManager<Doctor, DoctorDto, AddDoctorDto> baseManager, IDoctorManager doctorManager)
  {
    _baseManager = baseManager;
    _doctorManager = doctorManager;
  }


  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetDoctor(Guid id)
  {
    
    var doctor = await _baseManager.GetById(id);
    
    if (doctor == null)
    {
      return NotFound();
    }

    return Ok(doctor);
  }


  [HttpGet]
  public async Task<IActionResult> GetDoctors()
  {
     var doctors = await  _baseManager.GetAll();
      if (!doctors.Any())
      {
        return NotFound();
      }
      return Ok(doctors);
  }

  [HttpPost]
  public async Task<IActionResult> AddDoctor(AddDoctorDto doctor)
  {
    var createdDoctor = await _baseManager.Add(doctor);
    return CreatedAtAction(nameof(GetDoctor), new { id = createdDoctor.Id }, createdDoctor);
  }

  [HttpDelete("{id:guid}")]
  public IActionResult DeleteDoctor(Guid id)
  {
    _baseManager.Remove(id);
    return NoContent();
  }

  [HttpGet("search")]
  public async Task<IActionResult> SearchDoctors([FromQuery] string? term,
    CancellationToken cancellationToken = default)
  {
    var doctors = await _doctorManager.SearchByFullNameOrSpecialisation(term, cancellationToken);
    if (!doctors.Any())
    {
      return NotFound();
    }
    return Ok(doctors);
  }
}