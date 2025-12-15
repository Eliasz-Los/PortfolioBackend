using DAL.EntityFramework;
using DAL.Repository.hospital;
using Domain.hospital;

namespace BL.hospital;

public class AppointmentManager : BaseManager<Appointment>
{
    private readonly IAppointmentRepository _appointmentRepository;
    
    public AppointmentManager(IAppointmentRepository appointmentRepository) : base(appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsFromPatientById(Guid patientId)
    {
        return await _appointmentRepository.ReadAppointmentsByPatientId(patientId);
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsFromDoctorById(Guid doctorId)
    {
        return await _appointmentRepository.ReadAppointmentsByDoctorId(doctorId);
    }
}