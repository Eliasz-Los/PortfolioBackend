using BL.hospital.dto;
using Domain.hospital;

namespace BL.hospital;

public interface IAppointmentManager
{
    //TODO appointment dto
    public Task<IEnumerable<Appointment>> GetAllAppointmentsFromPatientById(Guid patientId);
    public Task<IEnumerable<Appointment>> GetAllAppointmentsFromDoctorById(Guid doctorId);

    public Task CompleteAppointment(Guid appointmentId);

    public Task<IEnumerable<DoctorAvailabilityDto>> GetDoctorAvailability(Guid doctorId, DateOnly from, DateOnly to);


}