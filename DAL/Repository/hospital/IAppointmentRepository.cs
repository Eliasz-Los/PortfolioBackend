using Domain.hospital;

namespace DAL.Repository.hospital;

public interface IAppointmentRepository : IBaseRepository<Appointment>
{
    Task<Appointment?> ReadAppointmentWithRelationsById(Guid appointmentId);
    Task<IEnumerable<Appointment>> ReadAppointmentsByPatientId(Guid patientId);
    Task<IEnumerable<Appointment>> ReadAppointmentsByDoctorId(Guid doctorId);
    Task<IEnumerable<Appointment>> ReadAppointmentsForDoctorInDateRange(Guid doctorId, DateTime from, DateTime to);
}