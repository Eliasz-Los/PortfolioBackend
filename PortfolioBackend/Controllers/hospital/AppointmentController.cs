using BL.hospital;
using Domain.hospital;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.hospital;

[ApiController]
[Route("api/hospital/")]
public class AppointmentController : Controller
{
    private readonly AppointmentManager _appointmentManager;

    private readonly IBaseManager<Appointment> _baseManager;
    
    public AppointmentController(AppointmentManager appointmentManager, IBaseManager<Appointment> baseManager)
    {
        _appointmentManager = appointmentManager;
        _baseManager = baseManager;
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
    
    
    
}