using BL.hospital;
using BL.hospital.dto;
using Domain.hospital;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/hospital/[controller]")]
public class AppointmentController : Controller
{
    private readonly AppointmentManager _appointmentManager;
    
    public AppointmentController(AppointmentManager appointmentManager)
    {
        _appointmentManager = appointmentManager;
    }

    [HttpGet("patient/{id}/appointments")]
    public async Task<IActionResult> GetPatientAppointments(Guid patientId)
    {
        var appointments =  await _appointmentManager.GetAllAppointmentsFromPatientById(patientId);

        if (!appointments.Any())
        {
            return NotFound();
        }

        return Ok(appointments);
    }

    [HttpGet("doctor/{id}/appointments")]
    public async Task<IActionResult> GetDoctorAppointments(Guid doctorId)
    {
        var appointments = await _appointmentManager.GetAllAppointmentsFromDoctorById(doctorId);

        if (!appointments.Any())
        {
            return NotFound();
        }
        
        return Ok(appointments);
    }
    
    
    [HttpPost("appointment")]
    public async  Task<IActionResult> CreateAppointment([FromBody] AddAppointmentDto appointmentDto)
    {
        var appointment = await _appointmentManager.Add(appointmentDto);
        return CreatedAtAction(nameof(CreateAppointment), new { id = appointment.Id }, appointment);
    }
    
    
    
}