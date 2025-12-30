using BL.hospital;
using BL.hospital.dto;
using Domain.hospital;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/hospital/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly AppointmentManager _appointmentManager;
    
    public AppointmentController(AppointmentManager appointmentManager)
    {
        _appointmentManager = appointmentManager;
    }

    [HttpGet("patients/{patientId:guid}")]
    public async Task<IActionResult> GetPatientAppointments(Guid patientId)
    {
        var appointments =  await _appointmentManager.GetAllAppointmentsFromPatientById(patientId);

        if (!appointments.Any())
        {
            return NotFound();
        }

        return Ok(appointments);
    }

    [HttpGet("doctors/{doctorId:guid}")]
    public async Task<IActionResult> GetDoctorAppointments( Guid doctorId)
    {
        var appointments = await _appointmentManager.GetAllAppointmentsFromDoctorById(doctorId);

        if (!appointments.Any())
        {
            return NotFound();
        }
        
        return Ok(appointments);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var appointment = await _appointmentManager.GetById(id);
        return appointment is null ? NotFound() : Ok(appointment);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAppointment(AddAppointmentDto appointmentDto)
    {
        var appointment = await _appointmentManager.Add(appointmentDto);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }
    
    
    
}