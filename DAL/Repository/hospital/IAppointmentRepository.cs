using Domain.hospital;

namespace DAL.Repository.hospital;

public interface IAppointmentRepository : IBaseRepository<Appointment>
{
    Task<IEnumerable<Appointment>> ReadAppointmentsByPatientId(Guid patientId);
    Task<IEnumerable<Appointment>> ReadAppointmentsByDoctorId(Guid doctorId);
}