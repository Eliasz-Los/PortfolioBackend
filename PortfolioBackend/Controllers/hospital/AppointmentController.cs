using BL.hospital;
using BL.hospital.dto;
using Domain.hospital;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/hospital/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly IBaseManager<Appointment, AppointmentDto, AddAppointmentDto> _appointmentManager;
    private readonly IAppointmentManager _appointmentManagerAddition;
    
    public AppointmentController(IBaseManager<Appointment, AppointmentDto, AddAppointmentDto> appointmentManager, IAppointmentManager appointmentManagerAddition)
    {
        _appointmentManager = appointmentManager;
        _appointmentManagerAddition = appointmentManagerAddition;
    }

    [HttpGet("patients/{patientId:guid}")]
    public async Task<IActionResult> GetPatientAppointments(Guid patientId)
    {
        var appointments =  await _appointmentManagerAddition.GetAllAppointmentsFromPatientById(patientId);

        if (!appointments.Any())
        {
            return NotFound();
        }

        return Ok(appointments);
    }

    [HttpGet("doctors/{doctorId:guid}")]
    public async Task<IActionResult> GetDoctorAppointments( Guid doctorId)
    {
        var appointments = await _appointmentManagerAddition.GetAllAppointmentsFromDoctorById(doctorId);

        if (!appointments.Any())
        {
            return NotFound();
        }
        
        return Ok(appointments);
    }
    
    [HttpGet("doctors/{doctorId:guid}/availability")]
    public async Task<IActionResult> GetDoctorAvailability(
        Guid doctorId,
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to)
    {
        var availability = await _appointmentManagerAddition
            .GetDoctorAvailability(doctorId, from, to);

        return Ok(availability);
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
    
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> CompleteAppointment(Guid id)
    {
       await _appointmentManagerAddition.CompleteAppointment(id);
       return NoContent();
    }
    
}